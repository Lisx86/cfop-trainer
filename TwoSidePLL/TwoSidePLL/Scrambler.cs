using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwoSidePLL
{
    class PLLManager {
        Dictionary<string/*taskName*/, string/*task command*/> taskMap = new Dictionary<string, string>();
        public PLLManager() {
            taskMap["H"] = "L F2 R2 F2 L' R' U2 R' B F' U2 B' F";
            taskMap["Ua"] = "R2 B2 U B2 U' B2 F2 D B2 D' F2 R2";
            taskMap["Ub"] = "L2 B R2 B' F D2 F U F2 L2 B2 D B2";
            taskMap["Z"] = "L U' F2 R U R' U' F2 L' U' L U2 L' U";
            taskMap["V"] = "B2 D' R D B2 U L U2 L2 B2 D' R D B2 L U2";
            taskMap["Y"] = "B2 U B2 U2 R2 U' R2 U' R2 U2 R2 B2 U' B2 U";
            taskMap["Na"] = "B U L2 U' D F' U' F2 D F' U F D2 L2 B'";
            taskMap["Nb"] = "L2 U2 L' B2 L2 U2 L' U2 B2 L2 D2 F2 R F2 D2";
            taskMap["E"] = "D' F D2 R D' B' D R' D2 F' D R B R' U";

            taskMap["Ga"] = "L2 R' D' R B2 R' D L2 R B F U2 B' F' U2";
            taskMap["Gb"] = "B' U2 B' F R2 D F' L2 F D' B2 F R2 F2 U";
            taskMap["Gc"] = "L2 U' B2 L D' R D2 L' D R' D2 B2 L2 U L2 U' U";
            taskMap["Gd"] = "L' R F2 R2 U2 R B2 U L U2 R' U' F2 R D2 U' U2";
            taskMap["Aa"] = "L F' L' F2 R2 F2 L F L' F2 R2 F2 U2";
            taskMap["Ab"] = "D2 L2 B2 L' F' L B2 L' F L' D2 U";
            taskMap["T"] = "R2 D' L2 B2 R D R' B2 L U' L U D R2";
            taskMap["Ja"] = "B2 L' B' L B' R' U' L U F U' F' L' R U'";
            taskMap["Jb"] = "F' L2 U L F' L B D' L' D F L B' F U'";
            taskMap["Ra"] = "B' U' R2 D2 L' F' L D2 R' B R' B' U B U2";
            taskMap["Rb"] = "F U' F2 D R2 B2 D' F' D B' U2 B' D' F2";
            taskMap["F"] = "L' U D2 F2 D' L D F2 U2 R U R' D2 L U'";
        }
        public string getTaskCommand(string taskName) {
            return taskMap[taskName];
        }
    }//cl
}//ns
