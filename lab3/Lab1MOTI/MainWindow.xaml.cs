using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Data;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Lab1MOTI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        private AppDbContext _context = new AppDbContext();

        public MainWindow()
        {
            InitializeComponent();
            _context.Database.EnsureCreated();
            LoadAllTables();
        }

        private void LoadAllTables()
        {
            _context.LPRs.Load();
            _context.Alternatives.Load();
            _context.Criterion.Load();

            _context.Results.Include(r => r.LPR).Include(r => r.Alternative).Load();
            _context.Vectors.Include(v => v.Alternative).Include(v => v.Criterion).Load();

            dgLRP.ItemsSource = _context.LPRs.Local.ToObservableCollection();
            dgAlt.ItemsSource = _context.Alternatives.Local.ToObservableCollection();
            dgCrit.ItemsSource = _context.Criterion.Local.ToObservableCollection();
            dgVectors.ItemsSource = _context.Vectors.Local.ToObservableCollection();
            dgResults.ItemsSource = _context.Results.Local.ToObservableCollection();

        }

        private void AddLpr_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditDialog();
            dialog.Title = "Add new LPR";
            dialog.lblSecond.Visibility = Visibility.Visible;
            dialog.txtSecond.Visibility = Visibility.Visible;
            dialog.lblFirst.Visibility = Visibility.Visible;
            dialog.lblFirst.Content = "LPR Name";
            dialog.lblSecond.Content = "Range";

            if (dialog.ShowDialog() == true)
            {
                string name = dialog.txtFirst.Text;
                if (int.TryParse(dialog.txtSecond.Text, out int range))
                {
                    _context.LPRs.Add(new LPR { LPRName = name, LPRRange = range });
                    _context.SaveChanges();
                }
            }
        }
        private void EditLpr_Click(object sender, RoutedEventArgs e)
        {
            if (dgLRP.SelectedItem is LPR selected)
            {
                var dialog = new EditDialog { Title = "Edit LPR" };
                dialog.lblFirst.Visibility = dialog.txtFirst.Visibility = Visibility.Visible;
                dialog.lblSecond.Visibility = dialog.txtSecond.Visibility = Visibility.Visible;
                dialog.txtFirst.Text = selected.LPRName;
                dialog.txtSecond.Text = selected.LPRRange.ToString();

                if (dialog.ShowDialog() == true)
                {
                    selected.LPRName = dialog.txtFirst.Text;
                    if (int.TryParse(dialog.txtSecond.Text, out int range)) selected.LPRRange = range;
                    _context.SaveChanges();
                    dgLRP.Items.Refresh();
                }
            }
        }
        private void DeleteLpr_Click(object sender, RoutedEventArgs e)
        {
            if (dgLRP.SelectedItem is LPR selected)
            {
                _context.LPRs.Remove(selected);
                _context.SaveChanges();
            }
        }
        private void AddAlternative_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditDialog();
            dialog.Title = "Add new Alternative";
            dialog.lblFirst.Visibility = Visibility.Visible;
            dialog.lblFirst.Content = "Alternative Name";

            if (dialog.ShowDialog() == true)
            {
                string name = dialog.txtFirst.Text;
                _context.Alternatives.Add(new Alternative { AlternativeName = name });
                _context.SaveChanges();
            }
        }
        private void EditAlternative_Click(object sender, RoutedEventArgs e)
        {
            if (dgAlt.SelectedItem is Alternative selected)
            {
                var dialog = new EditDialog { Title = "Edit Alternative" };
                dialog.lblFirst.Visibility = dialog.txtFirst.Visibility = Visibility.Visible;
                dialog.txtFirst.Text = selected.AlternativeName;

                if (dialog.ShowDialog() == true)
                {
                    selected.AlternativeName = dialog.txtFirst.Text;
                    _context.SaveChanges();
                    dgAlt.Items.Refresh();
                }
            }
        }
        private void DeleteAlternative_Click(object sender, RoutedEventArgs e)
        {
            if (dgAlt.SelectedItem is Alternative selected)
            {
                _context.Alternatives.Remove(selected);
                _context.SaveChanges();
            }
        }
        private void AddCriterion_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditDialog();
            dialog.Title = "Add new Criterion";
            dialog.lblFirst.Visibility = Visibility.Visible;
            dialog.lblFirst.Content = "Criterion Name";

            if (dialog.ShowDialog() == true)
            {
                string name = dialog.txtFirst.Text;
                _context.Criterion.Add(new Criterion { CriterionName = name });
                _context.SaveChanges();
            }
        }
        private void EditCriterion_Click(object sender, RoutedEventArgs e)
        {
            if (dgCrit.SelectedItem is Criterion selected)
            {
                var dialog = new EditDialog { Title = "Edit Criterion" };
                dialog.lblFirst.Visibility = dialog.txtFirst.Visibility = Visibility.Visible;
                dialog.txtFirst.Text = selected.CriterionName;

                if (dialog.ShowDialog() == true)
                {
                    selected.CriterionName = dialog.txtFirst.Text;
                    _context.SaveChanges();
                    dgCrit.Items.Refresh();
                }
            }
        }
        private void DeleteCriterion_Click(object sender, RoutedEventArgs e)
        {
            if (dgCrit.SelectedItem is Criterion selected)
            {
                _context.Criterion.Remove(selected);
                _context.SaveChanges();
            }
        }

        private void AddVector_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditDialog();
            dialog.Title = "Evaluate Alternative";
            dialog.lblFirst.Visibility = Visibility.Collapsed;
            dialog.txtFirst.Visibility = Visibility.Collapsed;

            dialog.lblCombo1.Visibility = Visibility.Visible;
            dialog.cbFirst.Visibility = Visibility.Visible;
            dialog.lblCombo1.Content = "Select Alternative";
            dialog.cbFirst.ItemsSource = _context.Alternatives.Local.ToObservableCollection();
            dialog.cbFirst.DisplayMemberPath = "AlternativeName";

            dialog.lblCombo2.Visibility = Visibility.Visible;
            dialog.cbSecond.Visibility = Visibility.Visible;
            dialog.lblCombo2.Content = "Select Criterion";
            dialog.cbSecond.ItemsSource = _context.Criterion.Local.ToObservableCollection();
            dialog.cbSecond.DisplayMemberPath = "CriterionName";

            dialog.lblSecond.Visibility = Visibility.Visible;
            dialog.txtSecond.Visibility = Visibility.Visible;
            dialog.lblSecond.Content = "Mark (Value or Qualitative)";

            if (dialog.ShowDialog() == true)
            {
                var alt = dialog.cbFirst.SelectedItem as Alternative;
                var crit = dialog.cbSecond.SelectedItem as Criterion;
                string mark = dialog.txtSecond.Text;

                if (alt != null && crit != null && !string.IsNullOrWhiteSpace(mark))
                {
                    _context.Vectors.Add(new Vector
                    {
                        AlternativeId = alt.AlternativeId,
                        CriterionId = crit.CriterionId,
                        Mark = mark
                    });
                    _context.SaveChanges();
                }
            }
        }
        private void EditVector_Click(object sender, RoutedEventArgs e)
        {
            if (dgVectors.SelectedItem is Vector selected)
            {
                var dialog = new EditDialog();

                dialog.Title = "Evaluate Alternative";
                dialog.lblFirst.Visibility = Visibility.Collapsed;
                dialog.txtFirst.Visibility = Visibility.Collapsed;

                dialog.lblCombo1.Visibility = Visibility.Visible;
                dialog.cbFirst.Visibility = Visibility.Visible;
                dialog.lblCombo1.Content = "Select Alternative";
                dialog.cbFirst.ItemsSource = _context.Alternatives.Local.ToObservableCollection();
                dialog.cbFirst.DisplayMemberPath = "AlternativeName";

                dialog.lblCombo2.Visibility = Visibility.Visible;
                dialog.cbSecond.Visibility = Visibility.Visible;
                dialog.lblCombo2.Content = "Select Criterion";
                dialog.cbSecond.ItemsSource = _context.Criterion.Local.ToObservableCollection();
                dialog.cbSecond.DisplayMemberPath = "CriterionName";

                dialog.lblSecond.Visibility = Visibility.Visible;
                dialog.txtSecond.Visibility = Visibility.Visible;
                dialog.lblSecond.Content = "Mark (Value or Qualitative)";
                dialog.cbFirst.SelectedItem = _context.Alternatives.Local.FirstOrDefault(a => a.AlternativeId == selected.AlternativeId);
                dialog.cbSecond.SelectedItem = _context.Criterion.Local.FirstOrDefault(c => c.CriterionId == selected.CriterionId);
                dialog.txtSecond.Text = selected.Mark;

                if (dialog.ShowDialog() == true)
                {
                    selected.AlternativeId = (dialog.cbFirst.SelectedItem as Alternative).AlternativeId;
                    selected.CriterionId = (dialog.cbSecond.SelectedItem as Criterion).CriterionId;
                    selected.Mark = dialog.txtSecond.Text;
                    _context.SaveChanges();
                    dgVectors.Items.Refresh();
                }
            }
        }

        private void DeleteVector_Click(object sender, RoutedEventArgs e)
        {
            if (dgVectors.SelectedItem is Vector selected)
            {
                _context.Vectors.Remove(selected);
                _context.SaveChanges();
            }
        }
        private void AddResult_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditDialog();
            dialog.Title = "Add new Result";
            dialog.lblCombo1.Visibility = Visibility.Visible;
            dialog.cbFirst.Visibility = Visibility.Visible;
            dialog.lblCombo1.Content = "Select LPR:";
            dialog.cbFirst.ItemsSource = _context.LPRs.Local.ToObservableCollection();
            dialog.cbFirst.DisplayMemberPath = "LPRName";

            dialog.lblFirst.Visibility = Visibility.Visible;
            dialog.txtFirst.Visibility = Visibility.Visible;
            dialog.lblFirst.Content = "LPR range:";
            dialog.txtFirst.IsReadOnly = true;

            dialog.cbFirst.SelectionChanged += (s, args) =>
            {
                if (dialog.cbFirst.SelectedItem is LPR selectedLpr)
                {
                    dialog.txtFirst.Text = selectedLpr.LPRRange.ToString();
                }
            };

            dialog.lblCombo2.Visibility = Visibility.Visible;
            dialog.cbSecond.Visibility = Visibility.Visible;
            dialog.lblCombo2.Content = "Select Alternative:";
            dialog.cbSecond.ItemsSource = _context.Alternatives.Local.ToObservableCollection();
            dialog.cbSecond.DisplayMemberPath = "AlternativeName";

            dialog.lblSecond.Visibility = Visibility.Visible;
            dialog.txtSecond.Visibility = Visibility.Visible;
            dialog.lblSecond.Content = "Alternative range:";

            if (dialog.ShowDialog() == true)
            {
                var selectedLpr = dialog.cbFirst.SelectedItem as LPR;
                var selectedAlt = dialog.cbSecond.SelectedItem as Alternative;

                if (selectedLpr != null && selectedAlt != null)
                {
                    if (int.TryParse(dialog.txtFirst.Text, out int lprRange) &&
                        int.TryParse(dialog.txtSecond.Text, out int altRange))
                    {
                        var newResult = new Result
                        {
                            LPRId = selectedLpr.LPRId,
                            LPRRange = lprRange,
                            AlternativeId = selectedAlt.AlternativeId,
                            AlternativeRange = altRange
                        };

                        _context.Results.Add(newResult);
                        _context.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show("Enter correct integers.");
                    }
                }
            }
        }
        private void EditResult_Click(object sender, RoutedEventArgs e)
        {
            if (dgResults.SelectedItem is Result selected)
            {
                var dialog = new EditDialog();
                dialog.Title = "Edit Result";
                dialog.lblCombo1.Visibility = Visibility.Visible;
                dialog.cbFirst.Visibility = Visibility.Visible;
                dialog.lblCombo1.Content = "Select LPR:";
                dialog.cbFirst.ItemsSource = _context.LPRs.Local.ToObservableCollection();
                dialog.cbFirst.DisplayMemberPath = "LPRName";
                dialog.cbFirst.SelectedItem = _context.LPRs.Local.FirstOrDefault(l => l.LPRId == selected.LPRId);

                dialog.lblFirst.Visibility = Visibility.Visible;
                dialog.txtFirst.Visibility = Visibility.Visible;
                dialog.lblFirst.Content = "LPR range:";
                dialog.txtFirst.IsReadOnly = true;
                dialog.txtFirst.Text = selected.LPRRange.ToString();

                dialog.cbFirst.SelectionChanged += (s, args) =>
                {
                    if (dialog.cbFirst.SelectedItem is LPR selectedLpr)
                    {
                        dialog.txtFirst.Text = selectedLpr.LPRRange.ToString();
                    }
                };

                dialog.lblCombo2.Visibility = Visibility.Visible;
                dialog.cbSecond.Visibility = Visibility.Visible;
                dialog.lblCombo2.Content = "Select Alternative:";
                dialog.cbSecond.ItemsSource = _context.Alternatives.Local.ToObservableCollection();
                dialog.cbSecond.DisplayMemberPath = "AlternativeName";
                dialog.cbSecond.SelectedItem = _context.Alternatives.Local.FirstOrDefault(a => a.AlternativeId == selected.AlternativeId);

                dialog.lblSecond.Visibility = Visibility.Visible;
                dialog.txtSecond.Visibility = Visibility.Visible;
                dialog.lblSecond.Content = "Alternative range:";
                dialog.txtSecond.Text = selected.AlternativeRange.ToString();

                if (dialog.ShowDialog() == true)
                {
                    var lpr = dialog.cbFirst.SelectedItem as LPR;
                    var alt = dialog.cbSecond.SelectedItem as Alternative;

                    if (lpr != null && alt != null &&
                        int.TryParse(dialog.txtFirst.Text, out int lRange) &&
                        int.TryParse(dialog.txtSecond.Text, out int aRange))
                    {
                        selected.LPRId = lpr.LPRId;
                        selected.AlternativeId = alt.AlternativeId;
                        selected.LPRRange = lRange;
                        selected.AlternativeRange = aRange;

                        _context.SaveChanges();
                        dgResults.Items.Refresh();
                    }
                }
            }
        }
        private void DeleteResult_Click(object sender, RoutedEventArgs e)
        {
            if (dgResults.SelectedItem is Result selected)
            {
                _context.Results.Remove(selected);
                _context.SaveChanges();
            }
        }

        public void DoEvaluation_Click(object sender, RoutedEventArgs e)
        {
            GeneratePairMatrix();
        }
        private void GeneratePairMatrix()
        {
            var lpr = dgLRP.SelectedItem as LPR;
            if (lpr == null)
            {
                MessageBox.Show("Choose LPR first!");
                return;
            }

            var alternatives = _context.Alternatives.ToList();
            int n = alternatives.Count;
            int[,] matrix = new int[n, n];

            
            var winCounts = alternatives.ToDictionary(a => a.AlternativeId, a => 0);

            DataTable dt = new DataTable();
            dt.Columns.Add("Alternative", typeof(string));

            for (int k = 0; k < n; k++)
            {
                dt.Columns.Add($"A{k + 1}", typeof(int));
            };

            for (int i = 0; i < n; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = alternatives[i].AlternativeName;

                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 1;
                        dr[j + 1] = 1;
                        continue;
                    }

                    bool result = CompareAlternatives(alternatives[i].AlternativeId, alternatives[j].AlternativeId);

                    string message = $"Expert Comparison Tool\n\n" +
                                     $"Option A: {alternatives[i].AlternativeName}\n" +
                                     $"Option B: {alternatives[j].AlternativeName}\n\n" +
                                     $"Is Option A WORSE than Option B?\n" +
                                     $"(Algorithm suggests: {(result ? "YES" : "NO")})";

                    var dialogResult = MessageBox.Show(message, "Expert Decision Support", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    bool finalDecision = (dialogResult == MessageBoxResult.Yes);

                    matrix[i, j] = finalDecision ? 1 : 0;
                    dr[j + 1] = matrix[i, j];

                    if (finalDecision) winCounts[alternatives[i].AlternativeId]++;
                }
                dt.Rows.Add(dr);
            }

            dgMatrix.ItemsSource = dt.DefaultView;
            SaveResultsToDb(lpr, winCounts);
        }

        private bool CompareAlternatives(int id1, int id2)
        {
            var marks1 = _context.Vectors.Where(v => v.AlternativeId == id1).ToList();
            var marks2 = _context.Vectors.Where(v => v.AlternativeId == id2).ToList();

            var weights = new Dictionary<string, double>
            {
                {"Час проходження маршруту",0.25 },
                {"Кількість зіткнень",0.3 },
                {"Ресурсна вартість", 0.15 },
                {"Стабільність поведінки", 0.2 },
                {"Складність реалізації", 0.1 }
            };
            double total1 = 0.0;
            double total2 = 0.0;

            foreach (var mark in marks1)
            {
                var criterion = _context.Criterion.Find(mark.CriterionId);
                if (criterion != null && weights.ContainsKey(criterion.CriterionName))
                {
                    total1 += double.Parse(mark.Mark) * weights[criterion.CriterionName];
                }
            }

            foreach (var mark in marks2)
            {
                var criterion = _context.Criterion.Find(mark.CriterionId);
                if (criterion != null && weights.ContainsKey(criterion.CriterionName))
                {
                    total2 += double.Parse(mark.Mark) * weights[criterion.CriterionName];
                }
            }

            return total1 < total2;
        }

        private void SaveResultsToDb(LPR lpr, Dictionary<int, int> winCounts)
        {
            var sorted = winCounts.OrderBy(x => x.Value).ToList();

            var oldResults = _context.Results.Where(r => r.LPRId == lpr.LPRId).ToList();
            if (oldResults.Any())
            {
                _context.Results.RemoveRange(oldResults);
            }

            for (int i = 0; i < sorted.Count; i++)
            {
                _context.Results.Add(new Result
                {
                    LPRId = lpr.LPRId,
                    LPRRange = lpr.LPRRange,
                    AlternativeId = sorted[i].Key,
                    AlternativeRange = i + 1 
                });
            }

            _context.SaveChanges();

            dgResults.Items.Refresh();
            MessageBox.Show("Ranking complete! Results have been saved to the database.", "Success");
        }

        private void viewVoting_Click(object sender, EventArgs e)
        {
            VotingProfile window = new VotingProfile();
            window.Show();
        }
    }
}