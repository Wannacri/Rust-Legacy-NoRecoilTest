string addr2 = "0x" + (await m.AoBScan("83 C4 10 8B 45 F4 D9 58 04 8D 47 64 D9 45 F8 D9 18 83 EC 0C")).FirstOrDefault().ToString("x1");
                Debug.WriteLine("byte recoil adresses: " + addr2.ToString());
                m.writeMemory(addr2.ToString(), "bytes", "83 C4 10 8B 45 F4 90 90 90 8D 47 64 D9 45 F8 90 90 83 EC 0C");