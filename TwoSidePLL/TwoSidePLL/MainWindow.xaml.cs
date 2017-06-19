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

namespace CfopTrainer { 
    public partial class MainWindow : Window {
        Cube theCube = new Cube();                       // the heart of the program (created once, and lives until the close)
        PLLManager practicer;                            // Pll practice manager
        Dictionary<Side, List<Polygon>>   uiData;        // Visible stickers set
        Dictionary<Side, List<Rectangle>> uiBackView;    // Invisible stickers set

        bool ControlsEnabled{ 
            set {
                cbCorrect.IsEnabled   = value;
                cbDiagonal.IsEnabled  = value;
                cbAdjacent.IsEnabled  = value;
                cbView.IsEnabled      = value;
                cbTopcolors.IsEnabled = value;

                btStart.IsEnabled         = value;
                comPracticeSize.IsEnabled = value;
                tbMovesTodo.IsEnabled     = value;
                btReset.IsEnabled         = value;
                btExecute.IsEnabled       = value;

                cbH.IsEnabled  = value;
                cbUa.IsEnabled = value;
                cbUb.IsEnabled = value;
                cbZ.IsEnabled  = value;

                cbV.IsEnabled  = value;
                cbY.IsEnabled  = value;
                cbNa.IsEnabled = value;
                cbNb.IsEnabled = value;
                cbE.IsEnabled  = value;

                cbGa.IsEnabled = value;
                cbGb.IsEnabled = value;
                cbGc.IsEnabled = value;
                cbGd.IsEnabled = value;
                cbAa.IsEnabled = value;
                cbAb.IsEnabled = value;
                cbT.IsEnabled  = value;
                cbJa.IsEnabled = value;
                cbJb.IsEnabled = value;
                cbRa.IsEnabled = value;
                cbRb.IsEnabled = value;
                cbF.IsEnabled  = value;

                cbLeft.IsEnabled   = value;
                cbRight.IsEnabled  = value;
                cbFace.IsEnabled   = value;
                cbBack.IsEnabled   = value;
                cbF2Lauf.IsEnabled = value;

                cbYellow.IsEnabled = value;
                cbWhite.IsEnabled  = value;
                cbRed.IsEnabled    = value;
                cbGreen.IsEnabled  = value;
                cbOrange.IsEnabled = value;
                cbBlue.IsEnabled   = value;
             }
        }
        public MainWindow() {
            InitializeComponent();

            // Initialize front view
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

            // initializeBackView
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
            updateGui();
        }

        // Transform checkboxes values into simple string lists
        internal List<string> getCheckedPLLcases() {
            var checkbs = new List<CheckBox>(){cbH, cbUa, cbUb, cbZ, cbV, cbY, cbNa, cbNb, cbE, cbGa, cbGb, cbGc, cbGd, cbAa, cbAb, cbT, cbJa, cbJb, cbRa, cbRb, cbF};
            var ret     = new List<string>();
            foreach(CheckBox cb in checkbs) {
                if(cb.IsChecked.Value) {
                    ret.Add(cb.Content.ToString());
                }
            }//for
            return ret;
        }
        internal List<string> getCheckedViews() {
            var views = new Dictionary<CheckBox, string/*AUF*/>(); // checkboxes-keys, tied to AUF-command strings
            views[cbFace]  = "";   //  0U noop   // TODO: these strings should perhaps be defined somewhere out of UI logic
            views[cbRight] = "U";  //  1U
            views[cbBack]  = "U2"; //  2U
            views[cbLeft]  = "U'"; //  1U'
           // views.Add(cbTurnF2L); // not needed, always 0..4 random

            var ret = new List<string/*AUF*/>();
            foreach (KeyValuePair<CheckBox, string> pair in views) {
                CheckBox cb = pair.Key;
                if (cb.IsChecked.Value) {
                    ret.Add(pair.Value);
                }
            }//for
            return ret;
        }
        internal List<Side>   getCheckedTotops() {
            var totops = new Dictionary<CheckBox, Side>(); // specific colors tied to the side were they must be turned
            totops.Add(cbYellow, Side.Up);
            totops.Add(cbWhite,  Side.Down);
            totops.Add(cbRed,    Side.Front);
            totops.Add(cbGreen,  Side.Right);
            totops.Add(cbOrange, Side.Back);
            totops.Add(cbBlue,   Side.Left);

            List<Side> ret = new List<Side>();
            foreach (KeyValuePair<CheckBox, Side> pair in totops) {
                CheckBox cb = pair.Key;
                if (cb.IsChecked.Value) {
                    ret.Add(pair.Value);
                }
            }//for
            return ret;
        }
        
         // Just transfers CUBE state into GUI
        private void updateGui() {
            Dictionary<Side, List<Color>> cubeData = theCube.getData();
            foreach (Side side in Enum.GetValues(typeof(Side))) {
                switch (side) {
                    case Side.Up:
                    case Side.Front:
                    case Side.Right:
                        if (cubeData[side].Count == uiData[side].Count){
                            for (int counter = 0; counter < cubeData[side].Count; counter++) {
                                uiData[side][counter].Fill = new SolidColorBrush(cubeData[side][counter]);
                            }
                        }
                        break;
                    case Side.Left:
                    case Side.Back:
                    case Side.Down:
                        if (cubeData[side].Count == uiBackView[side].Count) {
                            for (int counter = 0; counter < cubeData[side].Count; counter++){
                                uiBackView[side][counter].Fill = new SolidColorBrush(cubeData[side][counter]);
                            }
                        }
                        break;
                }//sw
            }//for          
        }//fn
        
        // Direct user interaction methods
        private void click_startPractice(object sender, RoutedEventArgs e) {
            if(practicer != null)
                return; // TODO: maybe a messagebox or something....

            // create new practicer instance
            // pass the selections to the practicer
            practicer = new PLLManager(theCube, getCheckedPLLcases(), getCheckedViews(), getCheckedTotops(), cbF2Lauf.IsChecked.Value);

            // decide the practice type in dependence of COMBO selection
            string selectedText = (comPracticeSize.SelectedItem as ComboBoxItem).Content.ToString();
            if (selectedText.Length > 3)
                // Practice all PLL cases 
                practicer.newPracticeAllcased();
            else
                // Practice only the selected cases, pass the combo value as GUESSING TIMES LIMIT
                practicer.newPractice( Convert.ToInt32(selectedText) );

            // generate next scramble
            if (practicer.nextScramble()) {
                // show the cube
                updateGui();
                
                // prepare progress bars and labels
                pbTasks.Minimum = 0;
                pbTasks.Maximum = practicer.TasksCount + 1;
                pbTasks.Value = 0;
                pbPenalties.Value = 0;
                pbTasks.Value = 0;
                lbRefine.Content = "";

                // Go!
                practicer.Running = true;
                ControlsEnabled   = false; // disable controls while guessing
            }//if
        }

        HashSet<Key> keysPressed = new HashSet<Key>();
        private void keyDown(object sender, KeyEventArgs e) {
            if(!keysPressed.Contains(e.Key)) {
                Title += "vv ";
                keysPressed.Add(e.Key);
                 switch (e.Key){
                    case Key.Left:  theCube.execute("y'");   updateGui();  break;
                    case Key.Right: theCube.execute("y");   updateGui();  break;
                }
            }

        }
        private void keyUp(object sender, KeyEventArgs e) {
            if(keysPressed.Contains(e.Key)) {
                keysPressed.Remove(e.Key);
                Title += "^^ ";
                switch (e.Key){
                    case Key.Left:  theCube.execute("y");   updateGui();  break;
                    case Key.Right: theCube.execute("y'");   updateGui();  break;
                }
            }//IF
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
        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {

        }

    }//cl
}//ns
