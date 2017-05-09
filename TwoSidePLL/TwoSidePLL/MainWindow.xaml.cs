using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace TwoSidePLL { 
    public partial class MainWindow : Window {
        Cube theCube;
        Practicer practicer;

        public MainWindow() {
            InitializeComponent();
            initializeFrontView();
            initializeBackView();
            initializeScrambles();
            initializeViews();
            initializeTotops();
            theCube = new Cube();
            practicer = new Practicer(this, theCube);
            updateGui();
        }
        private void click_startPractice(object sender, RoutedEventArgs e) {
            string selectedText = (comPracticeSize.SelectedItem as ComboBoxItem).Content.ToString();
            if (selectedText.Length > 3)
                practicer.newPracticeAllcased();
            else //                        practice length
                practicer.newPractice( Convert.ToInt32(selectedText) );

            // generate next scramble
            if (practicer.nextScramble()) {
                updateGui();                // show it
                practicer.Running = true;   // disable controls while guessing
            }
        }
        private void keyDown(object sender, KeyEventArgs e) {
            
            if (practicer.Running && e.Key.ToString().Length == 1) {
                char ch = char.ToLower(e.Key.ToString()[0]);
                Title += ch;  

                if (practicer.verify(ch)) { // verify 
                    pbTasks.Value++;
                     pbPenalties.Value = practicer.PenaltiesCount;
                } else {
                    pbTasks.Value++;
                    lbRefine.Content += " " + practicer.CurrentTask.getName();
                    if(pbPenalties.Maximum - pbPenalties.Value >= 3) {
                        // save the penality and update the penality bar
                        pbPenalties.Value = practicer.savePenalty(2); // adds penality: failed task +3 more;
                    }
                }//ei

                if(practicer.nextScramble()) {// go next guess
                    updateGui();               // show it
                } else {
                    practicer.Running = false;
                }//ei
            }//if
        }
        private void click_group_Correct(object sender, RoutedEventArgs e)
        {
            cbH.IsChecked = cbCorrect.IsChecked;
            cbUa.IsChecked = cbCorrect.IsChecked;
            cbUb.IsChecked = cbCorrect.IsChecked;
            cbZ.IsChecked = cbCorrect.IsChecked;
        }
        private void click_group_Diagonal(object sender, RoutedEventArgs e)
        {
            cbV.IsChecked = cbDiagonal.IsChecked;
            cbY.IsChecked = cbDiagonal.IsChecked;
            cbNa.IsChecked = cbDiagonal.IsChecked;
            cbNb.IsChecked = cbDiagonal.IsChecked;
            cbE.IsChecked = cbDiagonal.IsChecked;
        }
        private void click_group_Anjacent(object sender, RoutedEventArgs e)
        {
            cbGa.IsChecked = cbAdjacent.IsChecked;
            cbGb.IsChecked = cbAdjacent.IsChecked;
            cbGc.IsChecked = cbAdjacent.IsChecked;
            cbGd.IsChecked = cbAdjacent.IsChecked;
            cbAa.IsChecked = cbAdjacent.IsChecked;
            cbAb.IsChecked = cbAdjacent.IsChecked;
            cbT.IsChecked  = cbAdjacent.IsChecked;
            cbJa.IsChecked = cbAdjacent.IsChecked;
            cbJb.IsChecked = cbAdjacent.IsChecked;
            cbRa.IsChecked = cbAdjacent.IsChecked;
            cbRb.IsChecked = cbAdjacent.IsChecked;
            cbF.IsChecked  = cbAdjacent.IsChecked;
        }
        private void click_group_View(object sender, RoutedEventArgs e)
        {
            cbLeft.IsChecked    = cbView.IsChecked;
            cbRight.IsChecked   = cbView.IsChecked;
            cbBack.IsChecked    = cbView.IsChecked;
            cbFace.IsChecked    = cbView.IsChecked;
            //cbF2Lauf.IsChecked = cbView.IsChecked;
        }
        private void click_group_Topcolor(object sender, RoutedEventArgs e)
        {
            cbYellow.IsChecked = cbTopcolors.IsChecked;
            cbWhite.IsChecked  = cbTopcolors.IsChecked;
            cbRed.IsChecked    = cbTopcolors.IsChecked;
            cbGreen.IsChecked  = cbTopcolors.IsChecked;
            cbOrange.IsChecked = cbTopcolors.IsChecked;
            cbBlue.IsChecked   = cbTopcolors.IsChecked;
        }
        private void click_execute(object sender, RoutedEventArgs e)
        {
            Title = theCube.execute(tbMovesTodo.Text);
            updateGui();
        }
        private void click_reset(object sender, RoutedEventArgs e) {
            theCube.resetStickers(Side.Up);
            updateGui();
        }

    }//cl

    class Task : Pcase {
        Side totopSide;
        string auf;
        string f2lauf;
        public Task(string name, string todo, Side totopSide, string auf = "", string f2lauf = "")
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
    }

    partial class Practicer {
        Random random = new Random();
        Cube cube;
        PLLManager pllman = new PLLManager();
        Queue<Task> tasks     = new Queue<Task>();
        Queue<Task> penalties = new Queue<Task>();
        public Task CurrentTask { get; set; }

        public Practicer(MainWindow form, Cube cube) {
            this.form = form;
            this.cube = cube;
        }
        public void newPractice(int tasksToEnque) {
              for(/**/; tasksToEnque > 0; tasksToEnque--) {
                Task nextTask = generateRandomTask(
                      form.getSelectedScrambles()
                    , form.getSelectedViews()
                    , form.getSelectedTotops()
                );
                if(nextTask != null) 
                    tasks.Enqueue(nextTask);
            }//for
        }
        public void newPracticeAllcased() { // generate new set of tasks
            var selectedTotops = form.getSelectedTotops();
            int colIndx = random.Next(0, selectedTotops.Count);
            Side totopSide = selectedTotops.Count > 0 ? selectedTotops[colIndx] : Side.Up; // up by default

            List<Task> orderedTasks = new List<Task>();
            foreach(var selPcaseName in form.getSelectedScrambles()) {
                foreach(var selView in form.getSelectedViews()) {
                    Task nextTask = new Task(
                          selPcaseName
                        , pllman.getTaskCommand(selPcaseName)
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
            
            if (CurrentTask != null) {                             // check if got any tasks
                cube.resetStickers( CurrentTask.getTotopSide() );  // prepare the cube
                cube.execute( CurrentTask.getTodo() );             // rotate the cube
                return true; // got a task
            }
            return false; // got no more tasks
        }
        public bool verify(char ch) {
            return CurrentTask != null && CurrentTask.getName()[0] == ch.ToString().ToUpper()[0];
        }
        public int savePenalty(int deceptiveTasksCount) {
            penalties.Enqueue(CurrentTask);
            for(/**/; deceptiveTasksCount > 0; deceptiveTasksCount--) {
                Task nextTask = generateRandomTask(
                      form.getSelectedScrambles()
                    , form.getSelectedViews()
                    , new List<Side>(){ CurrentTask.getTotopSide() }
                );
                penalties.Enqueue(nextTask);
            }//for
            return penalties.Count;
        }
    }//cl
    
    public partial class MainWindow : Window {
        Dictionary<Side, List<Polygon>> uiData;
        Dictionary<Side, List<Rectangle>> uiBackView;
        List<CheckBox> scrambles = new List<CheckBox>(); // i think it should be a part of practicer
        Dictionary<CheckBox, string/*AUF*/> views = new Dictionary<CheckBox, string>();     // AUF for t
        Dictionary<CheckBox, Side> totops = new Dictionary<CheckBox, Side>(); //

        private void initializeFrontView()
        {
            uiData = new Dictionary<Side, List<Polygon>>();
            uiData.Add(Side.Front, new List<Polygon>());
            uiData.Add(Side.Right, new List<Polygon>());
            uiData.Add(Side.Up, new List<Polygon>());
            uiData[Side.Front].Add(face1);
            uiData[Side.Front].Add(face2);
            uiData[Side.Front].Add(face3);
            uiData[Side.Front].Add(face4);
            uiData[Side.Front].Add(face5);
            uiData[Side.Front].Add(face6);
            uiData[Side.Front].Add(face7);
            uiData[Side.Front].Add(face8);
            uiData[Side.Front].Add(face9);
            uiData[Side.Right].Add(side1);
            uiData[Side.Right].Add(side2);
            uiData[Side.Right].Add(side3);
            uiData[Side.Right].Add(side4);
            uiData[Side.Right].Add(side5);
            uiData[Side.Right].Add(side6);
            uiData[Side.Right].Add(side7);
            uiData[Side.Right].Add(side8);
            uiData[Side.Right].Add(side9);
            uiData[Side.Up].Add(ceil1);
            uiData[Side.Up].Add(ceil2);
            uiData[Side.Up].Add(ceil3);
            uiData[Side.Up].Add(ceil4);
            uiData[Side.Up].Add(ceil5);
            uiData[Side.Up].Add(ceil6);
            uiData[Side.Up].Add(ceil7);
            uiData[Side.Up].Add(ceil8);
            uiData[Side.Up].Add(ceil9);
        }
        private void initializeBackView()
        {
            uiBackView = new Dictionary<Side, List<Rectangle>>();
            uiBackView.Add(Side.Left, new List<Rectangle>());
            uiBackView.Add(Side.Back, new List<Rectangle>());
            uiBackView.Add(Side.Down, new List<Rectangle>());
            uiBackView[Side.Left].Add(left0);
            uiBackView[Side.Left].Add(left1);
            uiBackView[Side.Left].Add(left2);
            uiBackView[Side.Left].Add(left3);
            uiBackView[Side.Left].Add(left4);
            uiBackView[Side.Left].Add(left5);
            uiBackView[Side.Left].Add(left6);
            uiBackView[Side.Left].Add(left7);
            uiBackView[Side.Left].Add(left8);
            uiBackView[Side.Back].Add(back0);
            uiBackView[Side.Back].Add(back1);
            uiBackView[Side.Back].Add(back2);
            uiBackView[Side.Back].Add(back3);
            uiBackView[Side.Back].Add(back4);
            uiBackView[Side.Back].Add(back5);
            uiBackView[Side.Back].Add(back6);
            uiBackView[Side.Back].Add(back7);
            uiBackView[Side.Back].Add(back8);
            uiBackView[Side.Down].Add(down0);
            uiBackView[Side.Down].Add(down1);
            uiBackView[Side.Down].Add(down2);
            uiBackView[Side.Down].Add(down3);
            uiBackView[Side.Down].Add(down4);
            uiBackView[Side.Down].Add(down5);
            uiBackView[Side.Down].Add(down6);
            uiBackView[Side.Down].Add(down7);
            uiBackView[Side.Down].Add(down8);
        }
        private void initializeScrambles()
        {
            scrambles.Add(cbH);
            scrambles.Add(cbUa);
            scrambles.Add(cbUb);
            scrambles.Add(cbZ);

            scrambles.Add(cbV);
            scrambles.Add(cbY);
            scrambles.Add(cbNa);
            scrambles.Add(cbNb);
            scrambles.Add(cbE);

            scrambles.Add(cbGa);
            scrambles.Add(cbGb);
            scrambles.Add(cbGc);
            scrambles.Add(cbGd);
            scrambles.Add(cbAa);
            scrambles.Add(cbAb);
            scrambles.Add(cbT);
            scrambles.Add(cbJa);
            scrambles.Add(cbJb);
            scrambles.Add(cbRa);
            scrambles.Add(cbRb);
            scrambles.Add(cbF);
        }
        private void initializeViews() {
            views[cbFace]  = "";   //  0U noop
            views[cbRight] = "U";  //  1U
            views[cbBack]  = "U2"; //  2U
            views[cbLeft]  = "U'"; //  1U'
           // views.Add(cbTurnF2L); // not needed, always 0..4 random
        }
        private void initializeTotops() {
            totops.Add(cbYellow, Side.Up);
            totops.Add(cbWhite,  Side.Down);
            totops.Add(cbRed,    Side.Front);
            totops.Add(cbGreen,  Side.Right);
            totops.Add(cbOrange, Side.Back);
            totops.Add(cbBlue,   Side.Left);
        }
        internal List<string> getSelectedScrambles() {
            List<string> ret = new List<string>();
            foreach(CheckBox cb in scrambles) {
                if(cb.IsChecked.Value) {
                    ret.Add(cb.Content.ToString());
                }
            }//for
            return ret;
        }
        internal List<string/*AUF*/> getSelectedViews() {
            var ret = new List<string/*AUF*/>();
            foreach (KeyValuePair<CheckBox, string> pair in views) {
                CheckBox cb = pair.Key;
                if (cb.IsChecked.Value) {
                    ret.Add(pair.Value);
                }
            }//for
            return ret;
        }
        internal List<Side> getSelectedTotops() {
            List<Side> ret = new List<Side>();
            foreach (KeyValuePair<CheckBox, Side> pair in totops) {
                CheckBox cb = pair.Key;
                if (cb.IsChecked.Value) {
                    ret.Add(pair.Value);
                }
            }//for
            return ret;
        }
        private void updateGui() {
            Dictionary<Side, List<Color>> cubeData = theCube.getData();
            foreach (Side side in Enum.GetValues(typeof(Side)))
            {
                switch (side)
                {
                    case Side.Up:
                    case Side.Front:
                    case Side.Right:
                        if (cubeData[side].Count == uiData[side].Count)
                        {
                            for (int counter = 0; counter < cubeData[side].Count; counter++)
                            {
                                uiData[side][counter].Fill = new SolidColorBrush(cubeData[side][counter]);
                            }
                        }
                        break;
                    case Side.Left:
                    case Side.Back:
                    case Side.Down:
                        if (cubeData[side].Count == uiBackView[side].Count)
                        {
                            for (int counter = 0; counter < cubeData[side].Count; counter++)
                            {
                                uiBackView[side][counter].Fill = new SolidColorBrush(cubeData[side][counter]);
                            }
                        }
                        break;
                }//sw
            }//for          
        }
    }
    partial class Practicer {
        MainWindow form; // temporary. must use a delegate
        Stopwatch watch = Stopwatch.StartNew();
        bool m_running = false;
        public bool Running
        {
            get { return m_running; }
            set {
                if(value) {
                    watch = Stopwatch.StartNew();
                    form.pbTasks.Minimum = 0;
                    form.pbTasks.Maximum = TasksCount + 1;
                    form.pbTasks.Value = 0;

                    form.pbPenalties.Value = 0;
                } else {
                    watch.Stop();
                    string answer = string.Format("{0:D2}m:{1:D2}s:{2:D3}ms",
                                            watch.Elapsed.Minutes,
                                            watch.Elapsed.Seconds,
                                            watch.Elapsed.Milliseconds);
                    form.Title = answer;
                }
                m_running = value;
                form.cbCorrect.IsEnabled = !value;
                form.cbDiagonal.IsEnabled = !value;
                form.cbAdjacent.IsEnabled = !value;
                form.cbView.IsEnabled = !value;
                form.cbTopcolors.IsEnabled = !value;

                form.btStart.IsEnabled = !value;
                form.comPracticeSize.IsEnabled = !value;
                form.tbMovesTodo.IsEnabled = !value;
                form.btReset.IsEnabled = !value;
                form.btExecute.IsEnabled = !value;

                form.cbH.IsEnabled = !value;
                form.cbUa.IsEnabled = !value;
                form.cbUb.IsEnabled = !value;
                form.cbZ.IsEnabled = !value;

                form.cbV.IsEnabled = !value;
                form.cbY.IsEnabled = !value;
                form.cbNa.IsEnabled = !value;
                form.cbNb.IsEnabled = !value;
                form.cbE.IsEnabled = !value;

                form.cbGa.IsEnabled = !value;
                form.cbGb.IsEnabled = !value;
                form.cbGc.IsEnabled = !value;
                form.cbGd.IsEnabled = !value;
                form.cbAa.IsEnabled = !value;
                form.cbAb.IsEnabled = !value;
                form.cbT.IsEnabled = !value;
                form.cbJa.IsEnabled = !value;
                form.cbJb.IsEnabled = !value;
                form.cbRa.IsEnabled = !value;
                form.cbRb.IsEnabled = !value;
                form.cbF.IsEnabled = !value;

                form.cbLeft.IsEnabled = !value;
                form.cbRight.IsEnabled = !value;
                form.cbFace.IsEnabled = !value;
                form.cbBack.IsEnabled = !value;
                form.cbF2Lauf.IsEnabled = !value;

                form.cbYellow.IsEnabled = !value;
                form.cbWhite.IsEnabled = !value;
                form.cbRed.IsEnabled = !value;
                form.cbGreen.IsEnabled = !value;
                form.cbOrange.IsEnabled = !value;
                form.cbBlue.IsEnabled = !value;

                form.pbTasks.Value = 0;
                form.lbRefine.Content = "";
            }
        }
        public int TasksCount { get { return tasks.Count;  } }
        public int PenaltiesCount { get { return penalties.Count;  } }
        Task generateRandomTask(List<string> selectedPcases, List<string> selectedViews, List<Side> selectedTotops) {
            if(selectedPcases.Count == 0 || selectedViews.Count == 0 || selectedTotops.Count == 0) 
                return null;

            //AUF/AUF part             
            int aufIndx = random.Next(0, selectedViews.Count);
            string auf = " " + selectedViews[aufIndx]; // AUF

            // F2l-AUF is fixed 4 rotations max
            string auf2l = "";
            for(aufIndx = random.Next(0, 4); form.cbF2Lauf.IsChecked.Value && aufIndx > 0; aufIndx--) {
                auf2l += " d"; // add 0..3 turnd of 'd'
            }//for

            // Totopside
            int colIndx = random.Next(0, selectedTotops.Count);
            Side totopSide = selectedTotops[colIndx];

            // select the concrete PLL-case which has to be performed: (Aa, Z, Jb, Ra, Y, Nb, ...)
            int randomIndex = random.Next(0, selectedPcases.Count);
            string randomCaseName = selectedPcases[randomIndex];

            // create the task instance and enqueue it
            return new Task(randomCaseName, pllman.getTaskCommand(randomCaseName), totopSide, auf, auf2l);
        }
    }
}//ns
