using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics; // Stopwatch

namespace CfopTrainer {
    using CheckedPLLcases = List<string>;
    using CheckedViews    = List<string>;
    using CheckedTotops   = List<Side>;
    using TaskSpeciesMap = Dictionary<string/*Pll case name to execute*/, string/*Command for creating the pll case from clean cube*/>;

    class PLLManager: Practicer {
        TaskSpeciesMap taskMap   = new TaskSpeciesMap();
        Queue<PLLTask> tasks     = new Queue<PLLTask>();
        Queue<PLLTask> penalties = new Queue<PLLTask>();
        public PLLTask CurrentTask { get; set; }

        CheckedPLLcases checkedPllCases;
        CheckedViews    checkedViews;
        CheckedTotops   checkedTotops;
        bool turn_f2l;
        
        public PLLManager(Cube cube, CheckedPLLcases cases, CheckedViews views, CheckedTotops totops, bool f2auf):base(cube) {
            checkedPllCases = cases;
            checkedViews    = views;
            checkedTotops   = totops;
            turn_f2l        = f2auf;
            taskMap["H"]  = "L F2 R2 F2 L' R' U2 R' B F' U2 B' F";
            taskMap["Ua"] = "R2 B2 U B2 U' B2 F2 D B2 D' F2 R2";
            taskMap["Ub"] = "L2 B R2 B' F D2 F U F2 L2 B2 D B2";
            taskMap["Z"]  = "L U' F2 R U R' U' F2 L' U' L U2 L' U";
            taskMap["V"]  = "B2 D' R D B2 U L U2 L2 B2 D' R D B2 L U2";
            taskMap["Y"]  = "B2 U B2 U2 R2 U' R2 U' R2 U2 R2 B2 U' B2 U";
            taskMap["Na"] = "B U L2 U' D F' U' F2 D F' U F D2 L2 B'";
            taskMap["Nb"] = "L2 U2 L' B2 L2 U2 L' U2 B2 L2 D2 F2 R F2 D2";
            taskMap["E"]  = "D' F D2 R D' B' D R' D2 F' D R B R' U";
            taskMap["Ga"] = "L2 R' D' R B2 R' D L2 R B F U2 B' F' U2";
            taskMap["Gb"] = "B' U2 B' F R2 D F' L2 F D' B2 F R2 F2 U";
            taskMap["Gc"] = "L2 U' B2 L D' R D2 L' D R' D2 B2 L2 U L2 U' U";
            taskMap["Gd"] = "L' R F2 R2 U2 R B2 U L U2 R' U' F2 R D2 U' U2";
            taskMap["Aa"] = "L F' L' F2 R2 F2 L F L' F2 R2 F2 U2";
            taskMap["Ab"] = "D2 L2 B2 L' F' L B2 L' F L' D2 U";
            taskMap["T"]  = "R2 D' L2 B2 R D R' B2 L U' L U D R2";
            taskMap["Ja"] = "B2 L' B' L B' R' U' L U F U' F' L' R U'";
            taskMap["Jb"] = "F' L2 U L F' L B D' L' D F L B' F U'";
            taskMap["Ra"] = "B' U' R2 D2 L' F' L D2 R' B R' B' U B U2";
            taskMap["Rb"] = "F U' F2 D R2 B2 D' F' D B' U2 B' D' F2";
            taskMap["F"]  = "L' U D2 F2 D' L D F2 U2 R U R' D2 L U'";
        }
        public string getTaskCommand(string taskName) {
            return taskMap[taskName];
        }
 
        public void newPractice(int tasksToEnque) {
              for(/**/; tasksToEnque > 0; tasksToEnque--) {
                PLLTask nextTask = generateRandomTask(checkedPllCases, checkedViews, checkedTotops);
                if(nextTask != null) 
                    tasks.Enqueue(nextTask);
            }//for
        }
        public void newPracticeAllcased() { // generate new set of tasks
            int colIndx = random.Next(0, checkedTotops.Count);
            Side totopSide = checkedTotops.Count > 0 ? checkedTotops[colIndx] : Side.Up; // up by default

            List<PLLTask> orderedTasks = new List<PLLTask>();
            foreach(var selPcaseName in checkedPllCases) {
                foreach(var selView in checkedViews) {
                    PLLTask nextTask = new PLLTask(
                          selPcaseName
                        , getTaskCommand(selPcaseName)
                        , totopSide
                        , selView
                        , ""
                    );
                    orderedTasks.Add(nextTask);
                }//for
            }//for

            // randomize the tasks
            while(orderedTasks.Count > 0) {
                var randIndex = random.Next(0, orderedTasks.Count);
                tasks.Enqueue( orderedTasks[randIndex] );
                orderedTasks.RemoveAt(randIndex);
            }
        }
        public bool nextScramble() {            
            CurrentTask =        tasks.Count > 0 ? tasks.Dequeue() 
                           : penalties.Count > 0 ? penalties.Dequeue()
                           : null;
            
            // TODO: is it ok?
            if (CurrentTask != null) {                             // check if got any tasks
                cube.resetStickers( CurrentTask.getTotopSide() );  // reset the stickers & orient the cube accordingly
                cube.execute( CurrentTask.getTodo() );             // perform the algorithm which creates the pll-case from solved cube
                
                // have a new task!
                return true; 
            }//if

            // got no more tasks
            return false; 
        }
        public bool verify(char ch) {
            return CurrentTask != null && CurrentTask.getName()[0] == ch.ToString().ToUpper()[0];
        }
        public int savePenalty(int deceptiveTasksCount) {
            // Enqueue the penalty
            penalties.Enqueue(CurrentTask);

            // It would be too simple just to practice the hard case
            // So here are several more guesses for deceptive purposes :)
            for(/**/; deceptiveTasksCount > 0; deceptiveTasksCount--) {
                PLLTask nextTask = generateRandomTask(
                      checkedPllCases                                // generate new random pll case from selections
                    , checkedViews                                   // generate new random view
                    , new List<Side>(){ CurrentTask.getTotopSide() } // Let's use the same top color as the original task had
                );
                penalties.Enqueue(nextTask);
            }//for
            return penalties.Count;
        }

        bool m_running = false;
        public string LastRun {get; set;}
        public bool Running {
            get { return m_running; }
            set {
                if(value) {
                    watch = Stopwatch.StartNew();

                } else {
                    watch.Stop();
                    LastRun = string.Format("{0:D2}m:{1:D2}s:{2:D3}ms",
                                            watch.Elapsed.Minutes,
                                            watch.Elapsed.Seconds,
                                            watch.Elapsed.Milliseconds);
                }//ei
                m_running = value;
            }//set
        }
        public int TasksCount { get { return tasks.Count;  } }
        public int PenaltiesCount { get { return penalties.Count;  } }
        PLLTask generateRandomTask(List<string> selectedPcases, List<string> selectedViews, List<Side> selectedTotops) {
            if(selectedPcases.Count == 0 || selectedViews.Count == 0 || selectedTotops.Count == 0) 
                return null;

            //AUF/AUF part             
            int aufIndx = random.Next(0, selectedViews.Count);
            string auf = " " + selectedViews[aufIndx]; // AUF

            // F2l-AUF is fixed 4 rotations max
            string auf2l = "";
            for(aufIndx = random.Next(0, 4); turn_f2l && aufIndx > 0; aufIndx--) {
                auf2l += " d"; // add 0..3 turnd of 'd'
            }//for

            // Totopside
            int colIndx = random.Next(0, selectedTotops.Count);
            Side totopSide = selectedTotops[colIndx];

            // select the concrete PLL-case which has to be performed: (Aa, Z, Jb, Ra, Y, Nb, ...)
            int randomIndex = random.Next(0, selectedPcases.Count);
            string randomCaseName = selectedPcases[randomIndex];

            // create the task instance and enqueue it
            return new PLLTask(randomCaseName, getTaskCommand(randomCaseName), totopSide, auf, auf2l);
        }
    }//cl

     class PLLCase {
        string name; // the name of pll case
        string todo; // instrucction how to create it from clear cube
        public PLLCase(string name, string todo) {
            this.name = name;
            this.todo = todo;
        }
        public string getName() {
            return name;
        }
        virtual public string getTodo(){
            return todo;
        }
    }//cl

    class PLLTask : PLLCase {
        Side totopSide;
        string auf;
        string f2lauf;
        public PLLTask(string name, string todo, Side totopSide, string auf = "", string f2lauf = "")
        : base(name, todo) {
            this.totopSide = totopSide;
            this.auf = auf;
            this.f2lauf = f2lauf;
        }
        public override string getTodo() {
            return base.getTodo() + " " + auf + " " + f2lauf;
        }
        public Side getTotopSide() {
            return totopSide;
        }
    }//cl
}//ns
