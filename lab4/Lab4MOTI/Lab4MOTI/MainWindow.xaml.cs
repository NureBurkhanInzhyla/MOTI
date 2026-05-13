using System.Data;
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

namespace Lab4MOTI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();

        int[,] matrix =
        {
            {1,4,6},
            {3,7,8},
            {5,3,2}
        };

        string[] player = { "Атака", "Елементальний скіл", "Ухилення" };
        string[] boss = { "Сильна атака", "Слабка атака", "Заряд" };
        public MainWindow()
        {
            InitializeComponent();
            LoadMatrix();
        }
        void LoadMatrix()
        {
            DataTable dt = new DataTable();
            foreach (var item in boss)
            {
                dt.Columns.Add(item);
            }
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    dr[j] = matrix[i, j];
                }
                dt.Rows.Add(dr);
            }

            GameGrid.ItemsSource = dt.DefaultView;
            GameGrid.LoadingRow += (s, e) =>
            {
                e.Row.Header = player[e.Row.GetIndex()];
            };
        }
        void Play(int playerMove)
        {
            int bossMove = random.Next(0, 3);
            int result = matrix[playerMove, bossMove];
            ResultText.Text = $"Ви вибрали: {player[playerMove]}\nБос вибрав: {boss[bossMove]}\nВаш виграш: {result}";
            HighlightCell(playerMove, bossMove);
        }
        void HighlightCell(int row, int col)
        {

            GameGrid.SelectedCells.Clear();

            var item = GameGrid.Items[row];
            var column = GameGrid.Columns[col];

            DataGridCellInfo cellInfo = new DataGridCellInfo(item, column);
            GameGrid.SelectedCells.Add(cellInfo);
        }
        private void Attack_Click(object sender, RoutedEventArgs e) => Play(0);
        private void Skill_Click(object sender, RoutedEventArgs e) => Play(1);
        private void Dodge_Click(object sender, RoutedEventArgs e) => Play(2);
        private void Optimal_Click(object sender, RoutedEventArgs e)
        {
            int[] mins = new int[3];

            for (int i = 0; i < 3; i++)
                mins[i] = Enumerable.Range(0, 3).Min(j => matrix[i, j]);

            int maximin = mins.Max();
            int index = Array.IndexOf(mins, maximin);
            Play(index);

            ResultText.Text +=
                $"\nОптимальна стратегія: {player[index]} (maximin = {maximin})";
        }
    }
}