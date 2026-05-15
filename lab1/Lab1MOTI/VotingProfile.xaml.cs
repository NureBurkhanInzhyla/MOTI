using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Formats.Asn1.AsnWriter;

namespace Lab1MOTI
{
    public partial class VotingProfile : Window
    {
        private AppDbContext _context = new AppDbContext();
        private Dictionary<(int, int), (int scoreA, int scoreB)> wins;

        public VotingProfile()
        {
            _context.Database.EnsureCreated();
            InitializeComponent();
            LoadVotingProfile();
            wins = new Dictionary<(int, int), (int scoreA, int scoreB)>();
            LoadAlternativesPairs();
        }

        private void LoadVotingProfile()
        {
            var voters = _context.LPRs.OrderBy(v => v.LPRRange).ToList();
            var alternatives = _context.Alternatives.ToList();
            var results = _context.Results.ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Альтернатива", typeof(string));

            foreach (var voter in voters)
            {
                string header = $"{voter.LPRName} (R:{voter.LPRRange})";
                dt.Columns.Add(header, typeof(string));
            }
            foreach (var alternative in alternatives)
            {
                DataRow dr = dt.NewRow();
                dr[0] = alternative.AlternativeName;

                for (int j = 0; j < voters.Count; j++)
                {
                    var voter = voters[j];
                    var res = results.FirstOrDefault(r => r.LPRId == voter.LPRId && r.AlternativeId == alternative.AlternativeId);
                    if (res != null)
                    {
                        dr[j + 1] = res.AlternativeRange.ToString();
                    }
                    else
                    {
                        dr[j + 1] = "—";
                    }
                }
                dt.Rows.Add(dr);
            }

            dgVotingProfile.ItemsSource = dt.DefaultView;
          
        }
        public void btnCondorcet_Click(object sender, EventArgs e)
        {
            CalculateResultCondorcet();
        }

        private void LoadAlternativesPairs()
        {
            var voters = _context.LPRs.OrderBy(v => v.LPRRange).ToList();
            var alternatives = _context.Alternatives.ToList();
            var results = _context.Results.ToList();
            var orderedResults = results.ToDictionary(r => (r.LPRId, r.AlternativeId), r => r);

            wins.Clear();

            for (int i = 0; i < alternatives.Count; i++)
            {
                for (int j = i + 1; j < alternatives.Count; j++)
                {
                    var alternativeA = alternatives[i];
                    var alternativeB = alternatives[j];

                    int scoreA = 0;
                    int scoreB = 0;

                    foreach (var voter in voters)
                    {
                        int weight = (1 + voters.Count) - voter.LPRRange;
                        int rankA = orderedResults[(voter.LPRId, alternativeA.AlternativeId)].AlternativeRange;
                        int rankB = orderedResults[(voter.LPRId, alternativeB.AlternativeId)].AlternativeRange;

                        if (rankA < rankB) scoreA += weight;
                        else if (rankB < rankA) scoreB += weight;
                    }
                    wins[(alternativeA.AlternativeId, alternativeB.AlternativeId)] = (scoreA, scoreB);
                }
            }

        }
        private void CalculateResultCondorcet()
        {
            var alternatives = _context.Alternatives.ToList();
            string text = "Condorcet Duels:\n\n";
            Alternative winner = null;
            for (int i = 0; i < alternatives.Count; i++)
            {
                for (int j = i + 1; j < alternatives.Count; j++)
                {
                    var alternativeA = alternatives[i];
                    var alternativeB = alternatives[j];
                    var res = wins[(alternativeA.AlternativeId, alternativeB.AlternativeId)];
                    text += $"{alternativeA.AlternativeName} vs {alternativeB.AlternativeName} — {res.scoreA}:{res.scoreB}\n";

                }
            }

            foreach (var alternativeA in alternatives)
            {
                bool isWinner = true;

                foreach (var alternativeB in alternatives)
                {
                    if (alternativeA == alternativeB) continue;

                    int votesForA = 0;
                    int votesForB = 0;

                    if (alternativeA.AlternativeId < alternativeB.AlternativeId)
                    {
                        var res = wins[(alternativeA.AlternativeId, alternativeB.AlternativeId)];
                        votesForA = res.scoreA;
                        votesForB = res.scoreB;

                    }
                    else
                    {
                        var res = wins[(alternativeB.AlternativeId, alternativeA.AlternativeId)];
                        votesForA = res.scoreB;
                        votesForB = res.scoreA;
                    }

                    if (votesForA <= votesForB)
                    {
                        isWinner = false;
                        break;
                    }
                }

                if (isWinner)
                {
                    winner = alternativeA;
                }
            }
            if (winner != null)
            {
                text += $"\nFinal winner by Condorcet method: {winner.AlternativeName}\n";
            }
            else
            {
                text += "No Condorcet winner exists.";
            }

            MessageBox.Show(text, "Results");
        }
        private void btnCopeland_Click(object sender, EventArgs e)
        {
            CalculateResultCopeland();
        }
        private void CalculateResultCopeland()
        {
            var alternatives = _context.Alternatives.ToList();
            var scores = alternatives.ToDictionary(a => a, a => 0);

            foreach (var alternativeA in alternatives)
            {
                foreach (var alternativeB in alternatives)
                {
                    if (alternativeA == alternativeB) continue;

                    if (alternativeA.AlternativeId < alternativeB.AlternativeId)
                    {
                        var res = wins[(alternativeA.AlternativeId, alternativeB.AlternativeId)];
                        if (res.scoreA > res.scoreB) scores[alternativeA] += 1;
                        else if (res.scoreA < res.scoreB) scores[alternativeA] -= 1;
                    }
                    else
                    {
                        var res = wins[(alternativeB.AlternativeId, alternativeA.AlternativeId)];
                        if (res.scoreB > res.scoreA) scores[alternativeA] += 1;
                        else if (res.scoreB < res.scoreA) scores[alternativeA] -= 1;
                    }
                }
            }

            var result = scores.OrderByDescending(x => x.Value).ToList();
            string text = "Results by Copeland's method: \n";
            text += "Alternative - Points\n\n";
            foreach (var x in result)
            {
                text += $"{x.Key.AlternativeName}: {x.Value} points\n";
            }
            var winner = result.First();
            text += $"Final winner: {winner.Key.AlternativeName}";
            MessageBox.Show(text, "Results");
        }
     
    }
}
