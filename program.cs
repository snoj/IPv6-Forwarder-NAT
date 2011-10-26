using System;
using System.Net;
using System.Net.Sockets;

namespace us.snoj.GrowlIPForward {
	class Program {
		public static void Main(string[] args) {
			watch w = new watch();
			w.start();
			while(w.running) {
				System.Threading.Thread.Sleep(1);
			}
		}
	}
	
	class udpstate {
		public UdpClient u;
		public IPEndPoint e;
	}
	
	class watch {
		System.Threading.Thread t;
		udpstate s; // = new UdpClient(9887, AddressFamily.InterNetworkV6);
		public bool running { get { return t.IsAlive; } }
		public watch() {
			t = new System.Threading.Thread(new System.Threading.ThreadStart(loop));
			s = new udpstate();
			s.e = new IPEndPoint(IPAddress.IPv6Any, 9887);
			s.u = new UdpClient(s.e);
		}
		public void start() {
			t.Start();
		}
		public void stop() {
			t.Abort();
			s.u.Close();
		}
		protected void loop() {
			while(true) {
				IAsyncResult r = s.u.BeginReceive(receive, s);
				while(!r.IsCompleted) {
					System.Threading.Thread.Sleep(1);
				}
			}
		}
		
		protected void receive(IAsyncResult ar) {
			UdpClient u = (UdpClient)((udpstate)(ar.AsyncState)).u;
			IPEndPoint e = (IPEndPoint)((udpstate)(ar.AsyncState)).e;
			
			Byte[] receiveBytes = u.EndReceive(ar, ref e);
			Socket s4 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			s4.SendTo(receiveBytes, (EndPoint)new IPEndPoint(IPAddress.Any, 9887));
		}
	}
}