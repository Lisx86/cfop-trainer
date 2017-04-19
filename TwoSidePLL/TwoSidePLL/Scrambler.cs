using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwoSidePLL
{
    class CubeManip {
        public CubeManip() {
        }
        internal string scrambleTheCube(Cube cube, string newTaskTodo){
            switch(newTaskTodo) {
                case "H": cube.execute("L F2 R2 F2 L' R' U2 R' B F' U2 B' F"); return "H";
                case "Ua": cube.execute("R2 B2 U B2 U' B2 F2 D B2 D' F2 R2"); return "Ua";
                case "Ub": cube.execute("L2 B R2 B' F D2 F U F2 L2 B2 D B2"); return "Ub";
                case "Z": cube.execute("L U' F2 R U R' U' F2 L' U' L U2 L' U"); return "Z";
                case "V":  cube.execute("B2 D' R D B2 U L U2 L2 B2 D' R D B2 L U2"); return "V";
                case "Y":  cube.execute("B2 U B2 U2 R2 U' R2 U' R2 U2 R2 B2 U' B2 U"); return "Y";
                case "Na": cube.execute("B U L2 U' D F' U' F2 D F' U F D2 L2 B'"); return "Na";
                case "Nb": cube.execute("L2 U2 L' B2 L2 U2 L' U2 B2 L2 D2 F2 R F2 D2"); return "Nb";
                case "E":  cube.execute("D' F D2 R D' B' D R' D2 F' D R B R' U");  return "E";
                case "Ga": cube.execute("L2 R' D' R B2 R' D L2 R B F U2 B' F'"); return "Ga";
                case "Gb": cube.execute("B' U2 B' F R2 D F' L2 F D' B2 F R2 F2"); return "Gb";
                case "Gc": cube.execute("L2 U' B2 L D' R D2 L' D R' D2 B2 L2 U L2 U'"); return "Gc";
                case "Gd": cube.execute("L' R F2 R2 U2 R B2 U L U2 R' U' F2 R D2 U'"); return "Gd";
                case "Aa": cube.execute("L F' L' F2 R2 F2 L F L' F2 R2 F2"); return "Aa";
                case "Ab": cube.execute("D2 L2 B2 L' F' L B2 L' F L' D2"); return "Ab";
                case "T":  cube.execute("R2 D' L2 B2 R D R' B2 L U' L U D R2 U"); return "T";
                case "Ja": cube.execute("B2 L' B' L B' R' U' L U F U' F' L' R"); return "Ja";
                case "Jb": cube.execute("F' L2 U L F' L B D' L' D F L B' F"); return "Jb";
                case "Ra": cube.execute("B' U' R2 D2 L' F' L D2 R' B R' B' U B U2"); return "Ra";
                case "Rb": cube.execute("F U' F2 D R2 B2 D' F' D B' U2 B' D' F2 U"); return "Rb";
                case "F":  cube.execute("L' U D2 F2 D' L D F2 U2 R U R' D2 L"); return "F";
            }//sw
            return newTaskTodo;
        }
    }//cl
}//ns
