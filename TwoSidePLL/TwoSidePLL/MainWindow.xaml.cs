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
            theCube = new Cube();
            practicer = new Practicer(this, theCube);
            updateGui();
        }
        private void click_startPractice(object sender, RoutedEventArgs e) {
            int practiceLength = Convert.ToInt32((comPracticeSize.SelectedItem as ComboBoxItem).Content);
            practicer.newPractice(practiceLength);
            if (practicer.nextScramble())  {// generate next scramble
                updateGui();                // show it
                practicer.Running = true;
                pbCorrectAnswers.Minimum = 0;
                pbCorrectAnswers.Maximum = practicer.TasksCount+1;
                pbCorrectAnswers.Value = 0;
            }
        }
        private void keyDown(object sender, KeyEventArgs e) {
            
            if (practicer.Running && e.Key.ToString().Length == 1) {
                char ch = char.ToLower(e.Key.ToString()[0]);
                Title += ch;  

                if (practicer.verify(ch)) { // verify 
                    pbCorrectAnswers.Value++;
                } else {
                    lbRefine.Content += " " + practicer.TaskName.ToString();
                    practicer.savePenalty(); // adds penality: failed task +2 more
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
        private void click_reset(object sender, RoutedEventArgs e)
        {
            theCube.resetStickers();
            updateGui();
        }
    }//cl

    partial class Practicer {
        Cube cube;
        PLLManager pllman = new PLLManager();
        Queue<string> tasks     = new Queue<string>();
        Queue<Tuple<string, string, Side>> penalties = new Queue<Tuple<string, string, Side>>(); // Failed tasks command to execute
        //     taskname,  execcommand,  topcolor
        public Practicer(MainWindow form, Cube cube) {
            this.form = form;
            this.cube = cube;
        }
        public void newPractice(int tasksToGenerate)
        { // generate new set of tasks
            Random random = new Random();
            List<CheckBox> selections = form.getSelectedScrambles();
            if (selections.Count > 0) {
                for (/**/; tasksToGenerate > 0; tasksToGenerate--) {
                    int randomIndex = random.Next(0, selections.Count);
                    string text = selections[randomIndex].Content.ToString();
                    tasks.Enqueue(text);
                }//for
            }//if
        }
        public bool nextScramble() {            
            // got any tasks
            if (tasks.Count > 0) {
                var random = new Random();
                TaskName        = tasks.Dequeue(); // already randomed
                ExecutedCommand = pllman.getTaskCommand(TaskName);
                SideMovedTotop = Side.Up; // standard yellow side

                //AUF/AUF part
                var selectedViews = form.getSelectedViews();
                if (selectedViews.Count > 0) {
                    int aufIndx = random.Next(0, selectedViews.Count);
                    ExecutedCommand += " " + selectedViews[aufIndx]; // AUF
                }

                // F2l-AUF is fixed 4 rotations max
                if (form.cbF2Lauf.IsChecked.Value) {
                    ExecutedCommand += " d";
                    for(int aufIndx = random.Next(0, 3); aufIndx > 0; aufIndx--) {
                        ExecutedCommand += " d";
                    }//for
                }//if
                
                // Maint scramble, setup the actual PERM
                cube.resetStickers(); // TODO: reset to random color in dependence of the checkboxes
                cube.execute(ExecutedCommand); // generate the scramble;

               // form.Title = TaskName;
                return true; // got a task
            } else if(penalties.Count > 0)  {
                var tuple = penalties.Dequeue(); // already randomed
                TaskName        = tuple.Item1;
                ExecutedCommand = tuple.Item2;
                cube.resetStickers(); // TODO: reset to random color in dependence of the checkboxes
                cube.execute(ExecutedCommand); // generate the scramble;
                return true;
            }
            return false; // got no more tasks
        }
        public bool verify(char ch) {
            return ch.ToString().ToUpper()[0] == TaskName[0];
        }
        internal void savePenalty() {
            penalties.Enqueue(new Tuple<string, string, Side>(TaskName, ExecutedCommand, SideMovedTotop));

            
            List<CheckBox> selections = form.getSelectedScrambles();
            if (selections.Count > 0) {
                Random random = new Random();
                for (int tasksToGenerate = 2; tasksToGenerate > 0; tasksToGenerate--)
                {
                    int randomIndex = random.Next(0, selections.Count);
                    string text = selections[randomIndex].Content.ToString();
                    penalties.Enqueue(new Tuple<string, string, Side>(text, pllman.getTaskCommand(text), SideMovedTotop));
                }//for
            }//if
        
        }
    }//cl
    
    public partial class MainWindow : Window
    {
        Dictionary<Side, List<Polygon>> uiData;
        Dictionary<Side, List<Rectangle>> uiBackView;
        List<CheckBox> scrambles = new List<CheckBox>(); // i think it should be a part of practicer
        Dictionary<CheckBox, string/*AUF*/> views = new Dictionary<CheckBox, string>();     // AUF for t
        List<CheckBox> topColors = new List<CheckBox>(); //

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
        private void initializeViews()
        {
            views[cbFace]  = "U U'";   //  0U noop
            views[cbRight] = "U";  //  1U
            views[cbBack]  = "U2"; //  2U
            views[cbLeft]  = "U'"; //  1U'
           // views.Add(cbTurnF2L); // not needed, always 0..4 random
        }
        private void initializeTopcolors()
        {
            topColors.Add(cbYellow);
            topColors.Add(cbWhite);
            topColors.Add(cbRed);
            topColors.Add(cbGreen);
            topColors.Add(cbOrange);
            topColors.Add(cbBlue);
        }
        internal List<CheckBox> getSelectedScrambles()
        {
            List<CheckBox> ret = new List<CheckBox>();
            foreach(CheckBox cb in scrambles) {
                if(cb.IsChecked.Value) {
                    ret.Add(cb);
                }
            }//for
            return ret;
        }
        internal List<string/*AUF*/> getSelectedViews()
        {
            var ret = new List<string/*AUF*/>();
            foreach (KeyValuePair<CheckBox, string> pair in views) {
                CheckBox cb = pair.Key;
                if (cb.IsChecked.Value) {
                    ret.Add(pair.Value);
                }
            }//for
            return ret;
        }
        internal List<CheckBox> getSelectedColors()
        {
            List<CheckBox> ret = new List<CheckBox>();
            foreach (CheckBox cb in topColors) {
                if (cb.IsChecked.Value) {
                    ret.Add(cb);
                }
            }//for
            return ret;
        }
        private void updateGui()
        {
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
        bool m_running = false;
        public bool Running
        {
            get { return m_running; }
            set {
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

                form.pbCorrectAnswers.Value = 0;
                form.lbRefine.Content = "";
            }
        }
        internal string TaskName { get; set; }
        internal string ExecutedCommand { get; set; }
        internal Side SideMovedTotop { get; set; }
        internal int TasksCount { get { return tasks.Count;  } }
        MainWindow form; // temporary. must use a delegate
    }
}//ns
