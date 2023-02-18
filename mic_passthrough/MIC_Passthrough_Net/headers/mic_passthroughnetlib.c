#define _CRT_SECURE_NO_WARNINGS
//#define MA_DEBUG_OUTPUT
#define MINIAUDIO_IMPLEMENTATION
#define SAMPLE_RATE 48000
#define CHANNEL_COUNT 1
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#define EXPORT __declspec(dllexport)
#define M_PI 3.14159265358979323846264338327
#define FRAME_LENGTH 480
#include <WinSock.h>
#include <stdio.h>
#include <assert.h>
#include <time.h>
#include <stdio.h>
#include <stdlib.h>
#include <stddef.h>
#include <windows.h>
#include <tchar.h>
#include <stdint.h>
#include <stdbool.h>
#include <math.h>
#include <limits.h>
#include <conio.h>
#include <process.h>
#include "miniaudio.h"
#include "rnnoise.h"
#pragma comment(lib, "User32.lib")
//#pragma comment(lib, "RNNoiselib.lib")
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
EXPORT int startMicPassthrough_net(int, int);
/*EXPORT int retDevNameList_net(char* playbackCount, char* captureCount, char* playbackListGUI, char* captureListGUI, int len);
EXPORT void deinitAll_net();*/
EXPORT float getVadProbability_net();
EXPORT float getDecibel_net();
EXPORT bool transmitState_net();
EXPORT void setVolume_net(int volume);
//EXPORT float getAmplitude_net();
WSADATA wsaData;
SOCKET sockU, sock_control;
struct sockaddr_in server_addrU;
int bytes_sentU, retU;
float vadProbability = 0.0F, dB_avg = 0.0F, ampl_avg = 0.0F, volume = 1.0F;
bool prevCycleForwardDownState = false, prevCycleBackwardDownState = false, prevSendDownState = false, prevToggledState = false, prevCycleBackwardSoundDirDownState = false, prevVolUpState = false, prevVolDownState = false;
bool isToggled = false, isSendDown, isCycleForwardDown = false, isCycleBackwardDown = false, isVolUpDown = false, isVolDownDown = false, isPitchUpDown = false, isPitchDownDown = false, prevResetState = false;
SHORT sendKeyState = 0x7F, cycleForwardKeyState = 0x7F, cycleBackwardKeyState = 0x7F, disableKeyState = 0x7F, volUpKeyState = 0x7F, volDownKeyState = 0x7F, pitchUpKeyState = 0x7F, pitchDownKeyState = 0x7F;
uint32_t toggleKeyCode = 0x6B, voiceChatKeyCode, cycleForwardSoundKeyCode, cycleBackwardSoundKeyCode, toggleSoundboardKeyCode;
// TODO: Reimplement function to safely close out of threads and other tasks
DWORD WINAPI keyPressesThread(LPVOID lpParameter) {
  while (true) {
    sendKeyState = GetKeyState(voiceChatKeyCode), cycleForwardKeyState = GetKeyState(0x60), cycleBackwardKeyState = GetKeyState(0x61), disableKeyState = GetKeyState(toggleKeyCode);
    isToggled = disableKeyState & 1, isSendDown = sendKeyState & 0x8000, isCycleForwardDown = cycleForwardKeyState & 0x8000, isCycleBackwardDown = cycleBackwardKeyState & 0x8000;
    /*if (isToggled && isToggled != prevToggledState) {
      printf("[net]: Effects disabled!\n");
    } else if (isToggled == false && isToggled != prevToggledState) {
      printf("[net]: Effects enabled!\n");
    }*/
    Sleep(5);
    prevSendDownState = isSendDown;
    prevToggledState = isToggled;
    prevCycleForwardDownState = isCycleForwardDown;
    prevCycleBackwardDownState = isCycleBackwardDown;
  }
  return 0;
}
void setVolume_net(int vol) {
  volume = (float)vol / 100.0F;
}
bool transmitState_net() {
  return isToggled;
}
float get_average_decibel(short frames[], int length) {
  float dB_sum = 0.0;
  for (int i = 0; i < length; i++) {
    // Calculate the decibel level of the frame.
    float abs_sample = fabsf((float)frames[i]);
    float dB = 20.0f * log10f(abs_sample / 32768.0f);
    // Add the decibel level to the running total.
    dB_sum += dB;
  }
  // Calculate the average decibel level.
  float average_dB = dB_sum / length;
  return average_dB;
}
float get_average_amplitude(short frames[], int length) {
  float amplitude_sum = 0.0;
  for (int i = 0; i < length; i++) {
    // Convert the signed 16-bit value to a floating point value between -1 and 1.
    float sample = (float)frames[i] / 32768.0;
    // Take the absolute value of the sample to get the amplitude.
    float amplitude = fabsf(sample);
    // Add the amplitude to the running total.
    amplitude_sum += amplitude;
  }
  // Calculate the average amplitude.
  float average_amplitude = amplitude_sum / length;
  return average_amplitude;
}
float getVadProbability_net() {
  return vadProbability;
}
float getDecibel_net() {
  return dB_avg;
}
float getAmplitude_net() {
  return ampl_avg;
}
int netInitUDP(const char* host, int port) {
  sockU = INVALID_SOCKET;

  // Initialize Winsock
  retU = WSAStartup(MAKEWORD(2, 2), &wsaData);
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
int netDeinitUDP() {
  // Clean up
  closesocket(sockU);
  WSACleanup();
  return 0;
}
float noiseReductionProcessor(ma_device* pDevice, void* pOutput, const void* pInput, ma_uint32 frameCount) {
  if (st[0]) {
    short* pInputS16 = (short*)pInput;
    float* tempOut = calloc(frameCount * sizeof(float), sizeof(float));
    short* tempIn = calloc(frameCount * sizeof(short), sizeof(short));
    unsigned char* netOut = calloc(frameCount * sizeof(unsigned char), sizeof(unsigned char));
    for (int i = 0; i < frameCount; i++) tempOut[i] = (pInputS16[i]);
    //float fadeMultiplier = 1.0F, vadThreshold = 0.05F;
    float vadProb = rnnoise_process_frame(st[0], tempOut, tempOut);
    for (int i = 0; i < frameCount; i++) tempIn[i] = (tempOut[i] * volume);
    ma_convert_pcm_frames_format(pOutput, ma_format_s16, tempIn, ma_format_s16, (frameCount), 1, ma_dither_mode_none);
    ma_convert_pcm_frames_format(netOut, ma_format_u8, tempIn, ma_format_s16, (frameCount), 1, ma_dither_mode_none);
    netSendUDP(netOut, frameCount * sizeof(unsigned char));
    dB_avg = get_average_decibel(tempIn, frameCount);
    ampl_avg = get_average_amplitude(tempIn, frameCount);
    free(netOut);
    free(tempIn);
    free(tempOut);
    return vadProb;
  } else {
    printf("[net]: RNNoise = NULL!\n");
    exit(1);
  }
}
void* data_callback(ma_device* pDevice, void* pOutput, const void* pInput, ma_uint32 frameCount) {
  if (!transmitState_net()) {
    vadProbability = noiseReductionProcessor(pDevice, pOutput, pInput, frameCount);
  } else {
    vadProbability = 0.0F;
    dB_avg = 0.0F;
    ampl_avg = 0.0F;
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
    captureDevList[i] = -1;
  }
  for (int i = 0; i < initPlaybackDeviceCount; i++) {
    playbackDevList[i] = -1;
  }
}
void log_callback(ma_context* pContext, ma_device* pDevice, ma_uint32 logLevel, const char* message) {
  (void)pContext;
  (void)pDevice;
  (void)logLevel;
  printf("%s\n", message);
}
void stop_callback(ma_device* pDevice) {
  (void)pDevice;
  printf("[net]: STOPPED\n");
}
int initSound(bool isFromHelpPrompt, int **realmicDeviceId, int **virtmicDeviceId, int threadIndex) {
  ma_uint32 iDevice;
  ma_backend backend = ma_backend_wasapi;
  //backendRealMic = ma_backend_winmm;
  //backendRealMic = ma_backend_dsound;
  contextConfig[threadIndex] = ma_context_config_init();
  contextConfig[threadIndex].threadPriority = ma_thread_priority_realtime;

  ma_result result = ma_context_init(&backend, 1, &contextConfig[threadIndex], &context[threadIndex]);
  if (result != MA_SUCCESS) {
      printf("[net]: Failed to initialize context.\n");
      printf("[net]: DEBUG: %s\n", ma_result_description(result));
      return result;
  }

  ma_device_info* pCaptureDeviceInfosTmp;
  ma_device_info* pPlaybackDeviceInfosTmp;
  ma_uint32 captureDeviceCountTmp, playbackDeviceCountTmp;
  ma_result resultTmpDevEnum = ma_context_get_devices(&context[threadIndex], &pPlaybackDeviceInfosTmp, &playbackDeviceCountTmp, &pCaptureDeviceInfosTmp, &captureDeviceCountTmp);
  if (resultTmpDevEnum != MA_SUCCESS) {
    printf("[net]: FATAL: Failed to enumerate list of devices.\n");
    printf("[net]: DEBUG: %s\n", ma_result_description(resultTmpDevEnum));
    return resultTmpDevEnum;
  }
  pCaptureDeviceInfos[threadIndex] = (ma_device_info*)calloc(captureDeviceCountTmp, sizeof(ma_device_info));
  pPlaybackDeviceInfos[threadIndex] = (ma_device_info*)calloc(playbackDeviceCountTmp, sizeof(ma_device_info));
  ma_result resultDevEnum = ma_context_get_devices(&context[threadIndex], &pPlaybackDeviceInfos[threadIndex], &playbackDeviceCount[threadIndex], &pCaptureDeviceInfos[threadIndex], &captureDeviceCount[threadIndex]);
  if (resultDevEnum != MA_SUCCESS) {
    printf("[net]: FATAL: Failed to enumerate list of devices.\n");
    printf("[net]: DEBUG: %s\n", ma_result_description(resultDevEnum));
    return resultDevEnum;
  }
  printf("[net]: [REAL] microphone ID list:\n");
  for (iDevice = 0; iDevice < captureDeviceCount[threadIndex]; ++iDevice) {
    printf("    [%u]: \"%s\"\n", iDevice, pCaptureDeviceInfos[threadIndex][iDevice].name);
  }
  printf("[net]: [VIRTUAL] microphone ID list:\n");
  for (iDevice = 0; iDevice < playbackDeviceCount[threadIndex]; ++iDevice) {
    printf("    [%u]: \"%s\"\n", iDevice, pPlaybackDeviceInfos[threadIndex][iDevice].name);
  }
  if (**realmicDeviceId > -1 && **realmicDeviceId < captureDeviceCount[threadIndex]) {
    printf("[net]: Overrode [REAL] microphone ID: [%d] \"%s\"\n", **realmicDeviceId, pCaptureDeviceInfos[threadIndex][**realmicDeviceId].name);
  }
  if (**virtmicDeviceId > -1 && **virtmicDeviceId < playbackDeviceCount[threadIndex]) {
    printf("[net]: Overrode [VIRTUAL] microphone ID: [%d] \"%s\"\n", **virtmicDeviceId, pPlaybackDeviceInfos[threadIndex][**virtmicDeviceId].name);
  }
  if (**realmicDeviceId == -1) {
    printf("\n[net]: Defaulted [REAL] microphone ID to [0]\n");
    **realmicDeviceId = 0;
  }
  if (**virtmicDeviceId == -1) {
    printf("\n[net]: Defaulted [VIRTUAL] microphone ID to [0]\n");
    **virtmicDeviceId = 0;
  }
  return 0;
}
int spawnNewMiniaudioThread(int threadIndex, int deviceMode, int *realmicDeviceId, int *virtmicDeviceId) {
  assert(threadIndex < MAX_MINIAUDIO_THREADS && threadIndex > -1);
  assert(deviceMode <= 3 && deviceMode > -1);
  //st[threadIndex] = rnnoise_create(NULL);
  st[threadIndex] = rnnoise_create(&rnnoise_model_orig);
  if (initSound(true, &realmicDeviceId, &virtmicDeviceId, threadIndex)) {
    printf("[net]: FATAL: Error initializing sound devices..\n");
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
      deviceConfig[threadIndex].dataCallback = &data_callback;
      break;
    default:
      break;
  }
  deviceConfig[threadIndex].stopCallback = stop_callback;

  ma_result resultDevEnum = ma_device_init(&context[threadIndex], &deviceConfig[threadIndex], &device[threadIndex]);
  if (resultDevEnum != MA_SUCCESS) {
    printf("[net]: FATAL: Error initializing device..\n");
    printf("[net]: DEBUG: %s\n", ma_result_description(resultDevEnum));
    return -1;
  }

  Sleep(100);
  ma_result devStartRes = ma_device_start(&device[threadIndex]);
  if (devStartRes != MA_SUCCESS) {
    printf("[net]: FATAL: Error starting engine..\n");
    return -1;
  }
  return 0;
}
void deinitAll_net() {
  ma_device_uninit(&device[0]);
  rnnoise_destroy(st[0]);
  netDeinitUDP();
}
int startMicPassthrough_net(int captureDev, int playbackDev) {
  HANDLE hThread;
  DWORD dwThreadId;
  netInitUDP("192.168.168.170", 2224);
  int captureDevListCnt = 1;
  int playbackDevListCnt = 1;
  initSoundHardwareVars(captureDevListCnt, playbackDevListCnt);
  captureDevList[0] = captureDev;
  playbackDevList[0] = playbackDev;
  spawnNewMiniaudioThread(0, 0, &captureDevList[0], &playbackDevList[0]);
  hThread = CreateThread(NULL, 0, keyPressesThread, NULL, 0, &dwThreadId);
  if (hThread == NULL) {
    _tprintf(_T("[net]: CreateThread failed, error %d\n"), GetLastError());
    return 1;
  }
  while (true) {
    //fprintf(stderr, "VAD: %.4f\n", vadProbability);
    //fprintf(stderr, "dB: %f\n", get_average_decibel());
    //fprintf(stderr, "Amplitude: %f\n", get_average_amplitude() * 10000.0F);
    Sleep(500);
  }
  deinitAll_net();
  return 0;
}