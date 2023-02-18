using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Management;

namespace MicPassthroughAndRemoteMic {
    static class Program {
        public class MultiFormContext : ApplicationContext {
            private int openForms;
            public MultiFormContext(params Form[] forms) {
                openForms = forms.Length;
                foreach (var form in forms) {
                    form.FormClosed += (s, args) => {
                        //When we have closed the last of the "starting" forms, 
                        //end the program.
                        if (Interlocked.Decrement(ref openForms) == 0)
                            ExitThread();
                    };
                    form.Show();
                }
            }
        }
        [STAThread]
        public static void Main(string[] args) {
            // show GUI like
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MultiFormContext(new Form1()));
        }
    }
}