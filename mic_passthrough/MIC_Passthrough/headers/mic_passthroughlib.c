#define _CRT_SECURE_NO_WARNINGS
#define MA_DEBUG_OUTPUT
#define MINIAUDIO_IMPLEMENTATION
#define SAMPLE_RATE 48000
#define CHANNEL_COUNT 1
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#define EXPORT __declspec(dllexport)
#define M_PI 3.14159265358979323846264338327
#include <WinSock.h>
#include <stdio.h>
#include <assert.h>
#include <time.h>
#include <stdio.h>
#include <stdlib.h>
#include <stddef.h>
#include <windows.h>
#include <stdint.h>
#include <stdbool.h>
#include <math.h>
#include <limits.h>
#include <conio.h>
#include <process.h>
#include "miniaudio.h"
#include "rnnoise.h"
#pragma comment(lib, "User32.lib")
#pragma comment(lib, "RNNoiselib.lib")
#pragma comment(lib, "ws2_32.lib")
extern struct RNNModel rnnoise_model_orig;
extern struct RNNModel rnnoise_model_5h_b_500k;
extern struct RNNModel rnnoise_model_5h_ru_500k;
#define MAX_MINIAUDIO_THREADS 1
DenoiseState* st[MAX_MINIAUDIO_THREADS];
ma_context context[MAX_MINIAUDIO_THREADS];
ma_device device[MAX_MINIAUDIO_THREADS];
ma_device_config deviceConfig[MAX_MINIAUDIO_THREADS];
ma_context_config contextConfig[MAX_MINIAUDIO_THREADS];
ma_device_info **pPlaybackDeviceInfos;
ma_device_info **pCaptureDeviceInfos;
ma_uint32 *playbackDeviceCount, *captureDeviceCount;
int *captureDevList;
int *playbackDevList;
bool doRun = true;
EXPORT int startMicPassthrough(int, int);
EXPORT int dontRun();
EXPORT int shallRun();
EXPORT int getRunStatus();
EXPORT int retDevNameList(char* playbackCount, char* captureCount, char* playbackListGUI, char* captureListGUI, int len);
WSADATA wsaData;
SOCKET sock;
struct sockaddr_in server_addr;
int bytes_sent, ret;
WSADATA wsaDataU;
SOCKET sockU;
struct sockaddr_in server_addrU;
int bytes_sentU, retU;
int netInitTCP(const char* host, int port) {
  sock = INVALID_SOCKET;

  // Initialize Winsock
  ret = WSAStartup(MAKEWORD(2, 2), &wsaData);
  if (ret != 0) {
    printf("WSAStartup failed with error: %d\n", ret);
    return -1;
  }

  // Create a TCP socket
  sock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
  if (sock == INVALID_SOCKET) {
    printf("socket failed with error: %ld\n", WSAGetLastError());
    WSACleanup();
    return -1;
  }

  // Set server address
  server_addr.sin_family = AF_INET;
  server_addr.sin_port = htons(port);
  server_addr.sin_addr.s_addr = inet_addr(host);

  // Connect to server
  ret = connect(sock, (struct sockaddr*)&server_addr, sizeof(server_addr));
  if (ret == SOCKET_ERROR) {
    printf("connect failed with error: %d\n", WSAGetLastError());
    closesocket(sock);
    WSACleanup();
    return -1;
  }
  return 0;
}
int netSendTCP(void* frame, int frameSize) {
  bytes_sent = send(sock, (const char*)frame, frameSize, 0);
  if (bytes_sent == SOCKET_ERROR) {
    printf("send failed with error: %d\n", WSAGetLastError());
    closesocket(sock);
    WSACleanup();
    return -1;
  }
  return 0;
}
int netRecvTCP(void* frame, int frameSize) {
  // Receive response
  ret = recv(sock, frame, frameSize, 0);
  if (ret == SOCKET_ERROR) {
    printf("recv failed with error: %d\n", WSAGetLastError());
    closesocket(sock);
    WSACleanup();
    return -1;
  }
  return 0;
}
int netInitUDP(const char* host, int port) {
  sockU = INVALID_SOCKET;

  // Initialize Winsock
  retU = WSAStartup(MAKEWORD(2, 2), &wsaDataU);
  if (retU != 0) {
    printf("WSAStartup failed with error: %d\n", retU);
    return -1;
  }

  // Create a UDP socket
  sockU = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
  if (sockU == INVALID_SOCKET) {
    printf("socket failed with error: %ld\n", WSAGetLastError());
    WSACleanup();
    return -1;
  }

  // Set server address
  server_addrU.sin_family = AF_INET;
  server_addrU.sin_port = htons(port);
  server_addrU.sin_addr.s_addr = inet_addr(host);
  return 0;
}
int netSendUDP(void* frame, int frameSize) {
  bytes_sentU = sendto(sockU, (const char*)frame, frameSize, 0, (struct sockaddr*)&server_addrU, sizeof(server_addrU));
  if (bytes_sentU == SOCKET_ERROR) {
    printf("sendto failed with error: %d\n", WSAGetLastError());
    closesocket(sockU);
    WSACleanup();
    return -1;
  }
  return 0;
}
int netRecvUDP(void* frame, int frameSize) {
  retU = recvfrom(sockU, (const char*)frame, frameSize, 0, (struct sockaddr*)&server_addrU, sizeof(server_addrU));
  if (retU == SOCKET_ERROR) {
    printf("sendto failed with error: %d\n", WSAGetLastError());
    closesocket(sockU);
    WSACleanup();
    return -1;
  }
  return 0;
}
int netDeinit() {
  // Clean up
  closesocket(sock);
  WSACleanup();
  return 0;
}
int netDeinitU() {
  // Clean up
  closesocket(sockU);
  WSACleanup();
  return 0;
}
void *data_callback(ma_device* pDevice, void* pOutput, const void* pInput, ma_uint32 frameCount) {
  if (st[0]) {
    short* pInputS16 = (short*)pInput;
    float* tempOut = calloc(frameCount * sizeof(float), sizeof(float));
    short* tempIn = calloc(frameCount * sizeof(short), sizeof(short));
    for (int i = 0; i < frameCount; i++) tempOut[i] = pInputS16[i];
    //float fadeMultiplier = 1.0F, vadThreshold = 0.05F;
    float vadProbability = rnnoise_process_frame(st[0], tempOut, tempOut);
    for (int i = 0; i < frameCount; i++) tempIn[i] = tempOut[i];
    ma_convert_pcm_frames_format(pOutput, ma_format_s16, tempIn, ma_format_s16, (frameCount), 1, ma_dither_mode_none);
    //netSendTCP(tempIn, frameCount * sizeof(short));
    netSendUDP(tempIn, frameCount * sizeof(short));
    //netSendTCP(tempIn, frameCount * sizeof(short), 1);
    //netSendUDP(tempIn, frameCount * sizeof(short), 1);
    free(tempIn);
    free(tempOut);
  } else {
    printf("RNNoise = NULL!\n");
  }
}
void initSoundHardwareVars(int initCaptureDeviceCount, int initPlaybackDeviceCount) {
  // TODO: allow for definition of independent capture/playback device counts
  captureDevList = (int*)calloc(initCaptureDeviceCount, sizeof(int));
  playbackDevList = (int*)calloc(initPlaybackDeviceCount, sizeof(int));
  pCaptureDeviceInfos = (ma_device_info**)calloc(initCaptureDeviceCount, sizeof(ma_device_info*));
  pPlaybackDeviceInfos = (ma_device_info**)calloc(initPlaybackDeviceCount, sizeof(ma_device_info*));
  captureDeviceCount = (ma_uint32*)calloc(initCaptureDeviceCount, sizeof(ma_uint32));
  playbackDeviceCount = (ma_uint32*)calloc(initPlaybackDeviceCount, sizeof(ma_uint32));
  for (int i = 0; i < initCaptureDeviceCount; i++) {
    //printf("Assigning: -1 to capture index [%d]\n", i);
    captureDevList[i] = -1;
  }
  for (int i = 0; i < initPlaybackDeviceCount; i++) {
    //printf("Assigning: -1 to capture index [%d]\n", i);
    playbackDevList[i] = -1;
  }
  //printf("Initialized %d capture and %d playback device array(s)..\n", initCaptureDeviceCount, initPlaybackDeviceCount);
}
void log_callback(ma_context* pContext, ma_device* pDevice, ma_uint32 logLevel, const char* message) {
  (void)pContext;
  (void)pDevice;
  (void)logLevel;
  printf("%s\n", message);
}
void stop_callback(ma_device* pDevice) {
  (void)pDevice;
  printf("STOPPED\n");
}
bool isCaptureSoundDevAutoDetectable(int devEnum, int deviceCount, int threadIndex) {
  if (deviceCount == 1) {
    return false;
  }
  char *currCapDevName = pCaptureDeviceInfos[threadIndex][devEnum].name;
  if (strstr(currCapDevName, "AT2020USB+") != NULL) {
    printf("Auto-detected [REAL] microphone device ID: [%d] \"%s\"\n", devEnum, currCapDevName);
    return true;
  } /*else if (strstr(currCapDevName, "Line (MG-XU)") != NULL) {
    printf("Auto-detected [REAL] microphone device ID: [%d] \"%s\"\n", devEnum, currCapDevName);
    return true;
  }*/ else if (strstr(currCapDevName, "Microphone (Blue Snowball)") != NULL) {
    printf("Auto-detected [REAL] microphone device ID: [%d] \"%s\"\n", devEnum, currCapDevName);
    return true;
  } else if (strstr(currCapDevName, "Line In (High Definition Audio Device)") != NULL) {
    printf("Auto-detected [REAL] microphone device ID: [%d] \"%s\"\n", devEnum, currCapDevName);
    return true;
  } else if (strstr(currCapDevName, "Microphone (Sound Blaster Play! 3)") != NULL) {
    printf("Auto-detected [REAL] microphone device ID: [%d] \"%s\"\n", devEnum, currCapDevName);
    return true;
  } else {
    return false;
  }
}
bool isPlaybackSoundDevAutoDetectable(int devEnum, int deviceCount, int threadIndex) {
  if (deviceCount == 1) {
    return false;
  }
  char* currPlayDevName = pPlaybackDeviceInfos[threadIndex][devEnum].name;
  if (strstr(currPlayDevName, "VB-Cable Input (VB-Audio Virtual Cable)") != NULL) {
    printf("Auto-detected [VIRTUAL] microphone ID: [%d] \"%s\"\n", devEnum, currPlayDevName);
    return true;
  } else if (strstr(currPlayDevName, "Speakers (Sonics Virtual Audio Device (Wave Extensible) (WDM))") != NULL) {
    printf("Auto-detected [VIRTUAL] microphone ID: [%d] \"%s\"\n", devEnum, currPlayDevName);
    return true;
  /*} else if (strstr(currPlayDevName, "Speakers (High Definition Audio Device)") != NULL) {
    printf("Auto-detected [VIRTUAL] microphone ID: [%d] \"%s\"\n", devEnum, currPlayDevName);
    return true;*/
  } else if (strstr(currPlayDevName, "Line (MG-XU)") != NULL) {
    printf("Auto-detected [VIRTUAL] microphone ID: [%d] \"%s\"\n", devEnum, currPlayDevName);
    return true;
  } else {
    return false;
  }
}
int initSound(bool isFromHelpPrompt, int **realmicDeviceId, int **virtmicDeviceId, int threadIndex) {
  ma_uint32 iDevice;
  //ma_backend backend = ma_backend_alsa;
  ma_backend backend = ma_backend_wasapi;
  //backendRealMic = ma_backend_winmm;
  //backendRealMic = ma_backend_dsound;
  contextConfig[threadIndex] = ma_context_config_init();
  //contextConfig[threadIndex].logCallback = log_callback;
  contextConfig[threadIndex].threadPriority = ma_thread_priority_realtime;

  ma_result result = ma_context_init(&backend, 1, &contextConfig[threadIndex], &context[threadIndex]);
  if (result != MA_SUCCESS) {
      printf("Failed to initialize context.\n");
      printf("DEBUG: %s\n", ma_result_description(result));
      return result;
  }

  ma_device_info* pCaptureDeviceInfosTmp;
  ma_device_info* pPlaybackDeviceInfosTmp;
  ma_uint32 captureDeviceCountTmp, playbackDeviceCountTmp;
  ma_result resultTmpDevEnum = ma_context_get_devices(&context[threadIndex], &pPlaybackDeviceInfosTmp, &playbackDeviceCountTmp, &pCaptureDeviceInfosTmp, &captureDeviceCountTmp);
  if (resultTmpDevEnum != MA_SUCCESS) {
    printf("FATAL: Failed to enumerate list of devices.\n");
    printf("DEBUG: %s\n", ma_result_description(resultTmpDevEnum));
    return resultTmpDevEnum;
  }
  pCaptureDeviceInfos[threadIndex] = (ma_device_info*)calloc(captureDeviceCountTmp, sizeof(ma_device_info));
  pPlaybackDeviceInfos[threadIndex] = (ma_device_info*)calloc(playbackDeviceCountTmp, sizeof(ma_device_info));
  ma_result resultDevEnum = ma_context_get_devices(&context[threadIndex], &pPlaybackDeviceInfos[threadIndex], &playbackDeviceCount[threadIndex], &pCaptureDeviceInfos[threadIndex], &captureDeviceCount[threadIndex]);
  if (resultDevEnum != MA_SUCCESS) {
    printf("FATAL: Failed to enumerate list of devices.\n");
    printf("DEBUG: %s\n", ma_result_description(resultDevEnum));
    return resultDevEnum;
  }
  printf("[REAL] microphone ID list:\n");
  for (iDevice = 0; iDevice < captureDeviceCount[threadIndex]; ++iDevice) {
    printf("    [%u]: \"%s\"\n", iDevice, pCaptureDeviceInfos[threadIndex][iDevice].name);
  }
  printf("[VIRTUAL] microphone ID list:\n");
  for (iDevice = 0; iDevice < playbackDeviceCount[threadIndex]; ++iDevice) {
    printf("    [%u]: \"%s\"\n", iDevice, pPlaybackDeviceInfos[threadIndex][iDevice].name);
  }
  if (**realmicDeviceId > -1 && **realmicDeviceId < captureDeviceCount[threadIndex]) {
    printf("Overrode [REAL] microphone ID: [%d] \"%s\"\n", **realmicDeviceId, pCaptureDeviceInfos[threadIndex][**realmicDeviceId].name);
  }
  if (**virtmicDeviceId > -1 && **virtmicDeviceId < playbackDeviceCount[threadIndex]) {
    printf("Overrode [VIRTUAL] microphone ID: [%d] \"%s\"\n", **virtmicDeviceId, pPlaybackDeviceInfos[threadIndex][**virtmicDeviceId].name);
  }
  if (**realmicDeviceId >= captureDeviceCount[threadIndex] || **realmicDeviceId < 0) {
    for (iDevice = 0; iDevice < captureDeviceCount[threadIndex]; ++iDevice) {
      if (**realmicDeviceId == -1) {
        //printf("Checking [microphone]: %s\n", pCaptureDeviceInfos[threadIndex][iDevice].name);
        if (isCaptureSoundDevAutoDetectable(iDevice, captureDeviceCount[threadIndex], threadIndex)) {
          **realmicDeviceId = iDevice;
          iDevice = captureDeviceCount[threadIndex] * 2;
        }
      }
    }
  }
  if (**realmicDeviceId == -1) {
    printf("\nDefaulted [REAL] microphone ID to [0]\n");
    **realmicDeviceId = 0;
  }
  if (**virtmicDeviceId >= playbackDeviceCount[threadIndex] || **virtmicDeviceId < 0) {
    for (iDevice = 0; iDevice < playbackDeviceCount[threadIndex]; ++iDevice) {
      if (**virtmicDeviceId == -1) {
        //printf("Checking [playback]: %s\n", pPlaybackDeviceInfos[threadIndex][iDevice].name);
        if (isPlaybackSoundDevAutoDetectable(iDevice, captureDeviceCount[threadIndex], threadIndex)) {
          **virtmicDeviceId = iDevice;
          iDevice = playbackDeviceCount[threadIndex] * 2;
        }
      }
    }
  }
  if (**virtmicDeviceId == -1) {
    printf("\nDefaulted [VIRTUAL] microphone ID to [0]\n");
    **virtmicDeviceId = 0;
  }
  return 0;
}
int spawnNewMiniaudioThread(int threadIndex, int deviceMode, int *realmicDeviceId, int *virtmicDeviceId) {
  //initSoundHardwareVars(1, 1);
  assert(threadIndex < MAX_MINIAUDIO_THREADS && threadIndex > -1);
  assert(deviceMode <= 3 && deviceMode > -1);
  /*st[threadIndex] = rnnoise_create(NULL);*/
  st[threadIndex] = rnnoise_create(&rnnoise_model_orig);
  //st[threadIndex] = rnnoise_create(&rnnoise_model_5h_b_500k);
  //st[threadIndex] = rnnoise_create(&rnnoise_model_5h_ru_500k);
  if (initSound(true, &realmicDeviceId, &virtmicDeviceId, threadIndex)) {
    printf("FATAL: Error initializing sound devices..\n");
    return -1;
  }

  switch (deviceMode) {
    case 0:
      deviceConfig[threadIndex] = ma_device_config_init(ma_device_type_duplex);
      deviceConfig[threadIndex].capture.pDeviceID = &pCaptureDeviceInfos[threadIndex][*realmicDeviceId].id;
      deviceConfig[threadIndex].capture.format = ma_format_s16;
      deviceConfig[threadIndex].capture.channels = CHANNEL_COUNT;
      deviceConfig[threadIndex].capture.shareMode = ma_share_mode_shared;
      deviceConfig[threadIndex].playback.pDeviceID = &pPlaybackDeviceInfos[threadIndex][*virtmicDeviceId].id;
      deviceConfig[threadIndex].playback.format = ma_format_s16;
      deviceConfig[threadIndex].playback.channels = CHANNEL_COUNT;
      deviceConfig[threadIndex].playback.shareMode = ma_share_mode_shared;
      deviceConfig[threadIndex].sampleRate = SAMPLE_RATE;
      //deviceConfig[threadIndex].periodSizeInFrames = 1920;
      //deviceConfig[threadIndex].periodSizeInMilliseconds = 10;
      //deviceConfig[threadIndex].periods = 3;
      //deviceConfig[threadIndex].dataCallback = callbackFuncArr[threadIndex];
      deviceConfig[threadIndex].dataCallback = &data_callback;
      /*realFrameCount = deviceConfig[threadIndex].periodSizeInMilliseconds * (deviceConfig[threadIndex].sampleRate / 1000);
      sizeOfAudioFrame = (realFrameCount * ma_get_bytes_per_frame(deviceConfig[threadIndex].playback.format, deviceConfig[threadIndex].playback.channels));
      fftFrameCnt = (realFrameCount + nbits);
      numSampleCnt = (realFrameCount * deviceConfig[threadIndex].playback.channels);
      printf("realFrameCount: %d\nsizeOfAudioFrame: %d\n", realFrameCount, sizeOfAudioFrame);*/
      break;
    default:
      break;
  }
  //deviceConfig[threadIndex].noPreZeroedOutputBuffer = false;
  deviceConfig[threadIndex].stopCallback = stop_callback;

  ma_result resultDevEnum = ma_device_init(&context[threadIndex], &deviceConfig[threadIndex], &device[threadIndex]);
  if (resultDevEnum != MA_SUCCESS) {
    printf("FATAL: Error initializing device..\n");
    printf("DEBUG: %s\n", ma_result_description(resultDevEnum));
    return -1;
  }
  //printf("DEBUG: %s\n", ma_result_description(resultDevEnum));

  Sleep(100);
  ma_result devStartRes = ma_device_start(&device[threadIndex]);
  if (devStartRes != MA_SUCCESS) {
    printf("FATAL: Error starting engine..\n");
    return -1;
  }
  //printf("DEBUG: %s\n", ma_result_description(devStartRes));

  /*ma_channel_converter_config config = ma_channel_converter_config_init(
    ma_format_f32,                      // Sample format
    1,                              // Input channels
    NULL,                           // Input channel map
    1,                              // Output channels
    NULL,                           // Output channel map
    ma_channel_mix_mode_default);   // The mixing algorithm to use when combining channels.

  ma_result result = ma_channel_converter_init(&config, &converter);
  if (result != MA_SUCCESS) {
    // Error.
    printf("FATAL: Error initializing converter..\n");
    printf("DEBUG: %s\n", ma_result_description(resultDevEnum));
    return -1;
  }*/

  return 0;
}
int dontRun() {
  doRun = false;
  return 0;
}
int shallRun() {
  doRun = true;
  return 1;
}
int getRunStatus() {
  int runStatusCode = doRun;
  return runStatusCode;
}
size_t allocated_size(void* ptr) {
  return ((size_t*)ptr)[-1];
}
int retDevNameList(char* playbackCount, char* captureCount, char* playbackListGUI, char* captureListGUI, int len) {
  int captureDevListCnt = 1;
  int playbackDevListCnt = 1;
  initSoundHardwareVars(captureDevListCnt, playbackDevListCnt);
  captureDevList[0] = -1;
  playbackDevList[0] = -1;
  /*captureDevList[0] = 0;
  playbackDevList[0] = 0;*/
  spawnNewMiniaudioThread(0, 0, &captureDevList[0], &playbackDevList[0]);
  char* playbackDevListGUI[(256 + 1) * 32];
  char* captureDevListGUI[(256 + 1) * 32];
  char* tempStr[256 + 1];
  for (int i = 0; i < playbackDeviceCount[0]; i++) {
    sprintf((char*)tempStr, "%s\n", (const char*)&pPlaybackDeviceInfos[0][i].name);
    strncat((char*)playbackDevListGUI, (const char*)&tempStr, (size_t)len);
  }
  for (int i = 0; i < captureDeviceCount[0]; i++) {
    sprintf((char*)tempStr, "%s\n", (const char*)&pCaptureDeviceInfos[0][i].name);
    strncat((char*)captureDevListGUI, (const char*)&tempStr, (size_t)len);
  }
  _snprintf(playbackCount, (size_t)len, "%d", (int)playbackDeviceCount[0]);
  _snprintf(captureCount, (size_t)len, "%d", (int)captureDeviceCount[0]);
  _snprintf(playbackListGUI, (size_t)len, "%s\n", (char*)playbackDevListGUI);
  _snprintf(captureListGUI, (size_t)len, "%s\n", (char*)captureDevListGUI);
  /*fprintf(stderr, "\n%s\n", playbackDevListGUI);
  fprintf(stderr, "\n%s\n", captureDevListGUI);*/
  ma_device_uninit(&device[0]);
  rnnoise_destroy(st[0]);
  return 1;
}
void deinitAll() {
  ma_device_uninit(&device[0]);
  rnnoise_destroy(st[0]);
  shallRun();
  //netDeinit();
  netDeinitU();
}
int startMicPassthrough(int captureDev, int playbackDev) {
  //netInitTCP("192.168.168.170", 2224);
  netInitUDP("192.168.168.170", 2224);
  //netInitTCP("192.168.168.170", 2225, 1);
  //netInitUDP(ipAddr, portNum, 1);
  //captureDev = 1, playbackDev = 1;
  int captureDevListCnt = 1;
  int playbackDevListCnt = 1;
  initSoundHardwareVars(captureDevListCnt, playbackDevListCnt);
  captureDevList[0] = captureDev;
  playbackDevList[0] = playbackDev;
  spawnNewMiniaudioThread(0, 0, &captureDevList[0], &playbackDevList[0]);
  while (doRun) {
    // Do nothing, which is the normal procedure..
      Sleep(50000);
  }
  deinitAll();
  return 0;
}