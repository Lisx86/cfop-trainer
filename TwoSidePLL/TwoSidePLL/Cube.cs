using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CfopTrainer
{
    class Cube: Object{
        // there are 6 lists with 9 stickers each
        Dictionary<Side, List<Color>/*stickers list*/> data = new Dictionary<Side,List<Color>>();
        Dictionary<Side, Color> colorScheme = new Dictionary<Side,Color>();
        public Cube() {
            // make room for stickers
            data.Add(Side.Left,  new List<Color>());
            data.Add(Side.Front, new List<Color>());
            data.Add(Side.Right, new List<Color>());
            data.Add(Side.Back,  new List<Color>());
            data.Add(Side.Up,    new List<Color>());
            data.Add(Side.Down,  new List<Color>());

            // standard color scheme (set custom colors right here)
            colorScheme.Add(Side.Left,  Colors.Blue);
            colorScheme.Add(Side.Front, Colors.Red);
            colorScheme.Add(Side.Right, Colors.Green);
            colorScheme.Add(Side.Back,  Colors.DarkOrange);
            colorScheme.Add(Side.Up,    Colors.Yellow);
            colorScheme.Add(Side.Down,  Colors.White);

            // colorize the stickers
            resetStickers(Side.Up);
        }
        private void SwapStickers(Side lhsSide, int lhsIndex, Side rhsSide, int rhsIndex)
        {
            Color temp = data[lhsSide][lhsIndex];
            data[lhsSide][lhsIndex] = data[rhsSide][rhsIndex];
            data[rhsSide][rhsIndex] = temp;
        }
        internal Dictionary<Side, List<Color>> getData(){ 
            return data; //called only from GUI updater, no one else should get this
        }
        internal void resetStickers(Side totopSide) {
            data[Side.Left].Clear();
            data[Side.Front].Clear();
            data[Side.Right].Clear();
            data[Side.Back].Clear();
            data[Side.Up].Clear();
            data[Side.Down].Clear();

            // add 9-same-color-stickers to each of 6 sides
            for (int cnt = 0; cnt < 9; cnt++) {
                data[Side.Left]. Add(Colors.Blue);
                data[Side.Front].Add(Colors.Red);
                data[Side.Right].Add(Colors.Green);
                data[Side.Back]. Add(Colors.DarkOrange);
                data[Side.Up].   Add(Colors.Yellow);
                data[Side.Down]. Add(Colors.White);
            }

            switch(totopSide) {
                case Side.Front: execute("x"); break;
                case Side.Right: execute("z'"); break;
                case Side.Back:  execute("x'"); break;
                case Side.Left:  execute("z"); break;
                case Side.Down:  execute("x2"); break;
                case Side.Up: /* Do nothing. it's already there. */ break;
            }
        }
        internal void moveU()
        {
            SwapStickers(Side.Front, 0, Side.Right, 0);
            SwapStickers(Side.Right, 0, Side.Back, 2);
            SwapStickers(Side.Back, 2, Side.Left, 2);

            SwapStickers(Side.Front, 1, Side.Right, 1);
            SwapStickers(Side.Right, 1, Side.Back, 1);
            SwapStickers(Side.Back, 1, Side.Left, 1);

            SwapStickers(Side.Front, 2, Side.Right, 2);
            SwapStickers(Side.Right, 2, Side.Back, 0);
            SwapStickers(Side.Back, 0, Side.Left, 0);

            // Top stickers
            SwapStickers(Side.Up, 3, Side.Up, 7);
            SwapStickers(Side.Up, 7, Side.Up, 5);
            SwapStickers(Side.Up, 5, Side.Up, 1);
            SwapStickers(Side.Up, 0, Side.Up, 6);
            SwapStickers(Side.Up, 6, Side.Up, 8);
            SwapStickers(Side.Up, 8, Side.Up, 2);
        }
        internal void moveE()
        {
            // Stickers from the sides
            SwapStickers(Side.Front, 3, Side.Left, 5);
            SwapStickers(Side.Left, 5, Side.Back, 5);
            SwapStickers(Side.Back, 5, Side.Right, 3);

            SwapStickers(Side.Front, 4, Side.Left, 4);
            SwapStickers(Side.Left, 4, Side.Back, 4);
            SwapStickers(Side.Back, 4, Side.Right, 4);

            SwapStickers(Side.Front, 5, Side.Left, 3);
            SwapStickers(Side.Left, 3, Side.Back, 3);
            SwapStickers(Side.Back, 3, Side.Right, 5);

        }
        internal void moveD()
        {
            SwapStickers(Side.Right, 8, Side.Front, 8);
            SwapStickers(Side.Front, 8, Side.Left, 6);
            SwapStickers(Side.Left, 6 , Side.Back, 6);

            SwapStickers(Side.Right, 7, Side.Front, 7);
            SwapStickers(Side.Front, 7, Side.Left, 7);
            SwapStickers(Side.Left, 7, Side.Back, 7);

            SwapStickers(Side.Right, 6, Side.Front, 6);
            SwapStickers(Side.Front, 6, Side.Left, 8);
            SwapStickers(Side.Left, 8, Side.Back, 8);

            // BOttom stickers!
            SwapStickers(Side.Down, 8, Side.Down, 2);
            SwapStickers(Side.Down, 6, Side.Down, 8);
            SwapStickers(Side.Down, 0, Side.Down, 6);
            SwapStickers(Side.Down, 5, Side.Down, 1);
            SwapStickers(Side.Down, 7, Side.Down, 5);
            SwapStickers(Side.Down, 3, Side.Down, 7);
        }

        internal void moveR()
        {
            // Stickers from the sides
            SwapStickers(Side.Up, 2, Side.Front, 2);
            SwapStickers(Side.Front, 2, Side.Down, 8);
            SwapStickers(Side.Down, 8, Side.Back, 8);

            SwapStickers(Side.Up, 5, Side.Front, 5);
            SwapStickers(Side.Front, 5, Side.Down, 5);
            SwapStickers(Side.Down, 5, Side.Back, 5);

            SwapStickers(Side.Up, 8, Side.Front, 8);
            SwapStickers(Side.Front, 8, Side.Down, 2);
            SwapStickers(Side.Down, 2, Side.Back, 2);
            
            // RightOnly stickers
            SwapStickers(Side.Right, 3, Side.Right, 7);
            SwapStickers(Side.Right, 7, Side.Right, 5);
            SwapStickers(Side.Right, 5, Side.Right, 1);
            SwapStickers(Side.Right, 0, Side.Right, 6);
            SwapStickers(Side.Right, 6, Side.Right, 8);
            SwapStickers(Side.Right, 8, Side.Right, 2);
        }
        internal void moveM()
        {
            // Stickers from the sides
            SwapStickers(Side.Front, 1, Side.Up, 1);
            SwapStickers(Side.Up, 1, Side.Back, 7);
            SwapStickers(Side.Back, 7, Side.Down, 7);

            SwapStickers(Side.Front, 4, Side.Up, 4);
            SwapStickers(Side.Up, 4, Side.Back, 4);
            SwapStickers(Side.Back, 4, Side.Down, 4);

            SwapStickers(Side.Front, 7, Side.Up, 7);
            SwapStickers(Side.Up, 7, Side.Back, 1);
            SwapStickers(Side.Back, 1, Side.Down, 1);
        }
        internal void moveL()
        {
            // Stickers from the sides
            SwapStickers(Side.Front, 0, Side.Up, 0);
            SwapStickers(Side.Up, 0, Side.Back, 6);
            SwapStickers(Side.Back, 6, Side.Down, 6);

            SwapStickers(Side.Front, 3, Side.Up, 3);
            SwapStickers(Side.Up, 3, Side.Back, 3);
            SwapStickers(Side.Back, 3, Side.Down, 3);
            SwapStickers(Side.Front, 6, Side.Up, 6);
            SwapStickers(Side.Up, 6, Side.Back, 0);
            SwapStickers(Side.Back, 0, Side.Down, 0);

            // LeftOnly stickers
            SwapStickers(Side.Left, 7, Side.Left, 5);
            SwapStickers(Side.Left, 3, Side.Left, 7);
            SwapStickers(Side.Left, 1, Side.Left, 3);
            SwapStickers(Side.Left, 8, Side.Left, 2);
            SwapStickers(Side.Left, 6, Side.Left, 8);
            SwapStickers(Side.Left, 0, Side.Left, 6);
        }

        internal void moveF()
        {
            SwapStickers(Side.Right, 6, Side.Up, 8);
            SwapStickers(Side.Up, 8, Side.Left, 0);
            SwapStickers(Side.Left, 0, Side.Down, 6);

            SwapStickers(Side.Right, 3, Side.Up, 7);
            SwapStickers(Side.Up, 7, Side.Left, 3);
            SwapStickers(Side.Left, 3, Side.Down, 7);

            SwapStickers(Side.Right, 0, Side.Up, 6);
            SwapStickers(Side.Up, 6, Side.Left, 6);
            SwapStickers(Side.Left, 6, Side.Down, 8);

            // BottomOnly stickers
            SwapStickers(Side.Front, 3, Side.Front, 7);
            SwapStickers(Side.Front, 7, Side.Front, 5);
            SwapStickers(Side.Front, 5, Side.Front, 1);

            SwapStickers(Side.Front, 0, Side.Front, 6);
            SwapStickers(Side.Front, 6, Side.Front, 8);
            SwapStickers(Side.Front, 8, Side.Front, 2);
        }
        internal void moveS()
        {
            SwapStickers(Side.Right, 7, Side.Up, 5);
            SwapStickers(Side.Up, 5, Side.Left, 1);
            SwapStickers(Side.Left, 1, Side.Down, 3);

            SwapStickers(Side.Right, 4, Side.Up, 4);
            SwapStickers(Side.Up, 4, Side.Left, 4);
            SwapStickers(Side.Left, 4, Side.Down, 4);

            SwapStickers(Side.Right, 1, Side.Up, 3);
            SwapStickers(Side.Up, 3, Side.Left, 7);
            SwapStickers(Side.Left, 7, Side.Down, 5);
        }
        internal void moveB()
        {
            SwapStickers(Side.Up, 0, Side.Right, 2);
            SwapStickers(Side.Right, 2, Side.Down, 2);
            SwapStickers(Side.Down, 2, Side.Left, 8);
            SwapStickers(Side.Up, 1, Side.Right, 5);
            SwapStickers(Side.Right, 5, Side.Down, 1);
            SwapStickers(Side.Down, 1, Side.Left, 5);
            SwapStickers(Side.Up, 2, Side.Right, 8);
            SwapStickers(Side.Right, 8, Side.Down, 0);
            SwapStickers(Side.Down, 0, Side.Left, 2);

            // BottomOnly stickers
            SwapStickers(Side.Back, 8, Side.Back, 2);
            SwapStickers(Side.Back, 6, Side.Back, 8);
            SwapStickers(Side.Back, 0, Side.Back, 6);
            SwapStickers(Side.Back, 5, Side.Back, 1);
            SwapStickers(Side.Back, 7, Side.Back, 5);
            SwapStickers(Side.Back, 3, Side.Back, 7);
        }

        internal void moveUprime()
        {
            moveU();
            moveU();
            moveU();
        }
        internal void moveEprime()
        {
            moveE();
            moveE();
            moveE();
        }
        internal void moveDprime()
        {
            moveD();
            moveD();
            moveD();
        }

        internal void moveRprime()
        {
            moveR();
            moveR();
            moveR();
        }
        internal void moveMprime()
        {
            moveM();
            moveM();
            moveM();
        }
        internal void moveLprime()
        {
            moveL();
            moveL();
            moveL();
        }

        internal void moveFprime()
        {
            moveF();
            moveF();
            moveF();
        }
        internal void moveSprime()
        {
            moveS();
            moveS();
            moveS();
        }
        internal void moveBprime()
        {
            moveB();
            moveB();
            moveB();
        }

        internal string execute(string movesList){
            string formattedMoves = "";
            string[] tokens = movesList.Split(new char[] { ' ', ',', '.', ':', '\t', '(', ')', '[', ']', '{', '}' });

            foreach (string item in tokens) {
                if (item.Length != 0) {
                    formattedMoves += item + " ";

                    bool doubleTurn = item.Length > 1 && item[1] == '2';
                    bool prime = (item.Length == 2 && item[1] == '\'') || (item.Length == 3 && item[2] == '\'');

                    if (!doubleTurn && !prime) { // simple move
                        atomicMove(item[0]);
                    } else if (!doubleTurn && prime) {// prime
                        atomicMove(item[0]);
                        atomicMove(item[0]);
                        atomicMove(item[0]);
                    } else if (doubleTurn) {
                        atomicMove(item[0]);
                        atomicMove(item[0]);
                    }//ei
                }//if
            }//for
            return formattedMoves;
        }
        internal string atomicMove(char moveType) {
            bool doubleSlice = moveType >= 'a' && moveType <= 'z';
            char ch = char.ToLower(moveType);

            if (!doubleSlice)
                switch (ch) {
                    case 'u': moveU(); break;
                    case 'e': moveE(); break;
                    case 'd': moveD(); break;
                    case 'r': moveR(); break;
                    case 'm': moveM(); break;
                    case 'l': moveL(); break;
                    case 'f': moveF(); break;
                    case 's': moveS(); break;
                    case 'b': moveB(); break;
                    default: return "Wrong move type: " + ch;
                }//sw

            if (doubleSlice)
                switch (ch) {
                    case 'u': moveU(); moveEprime(); break;
                    case 'e': moveE(); moveEprime(); break;
                    case 'd': moveD(); moveE(); break;
                    case 'r': moveR(); moveMprime(); break;
                    case 'm': moveM(); moveEprime(); break;
                    case 'l': moveL(); moveM(); break;
                    case 'f': moveF(); moveS(); break;
                    case 's': moveS(); moveEprime(); break;
                    case 'b': moveB(); moveSprime(); break;
                    case 'x': moveR(); moveMprime(); moveLprime(); break;
                    case 'y': moveU(); moveEprime(); moveDprime(); break;
                    case 'z': moveF(); moveS(); moveBprime(); break;
                    default:  return "Wrong move type: " + ch;
                }//sw
            return "ok";
        }
    }//cl
}//ns
