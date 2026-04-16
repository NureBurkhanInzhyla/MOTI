using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1MOTI
{
    public class Alternative
    {
        [Key]
        [Column("alternative_id")]
        public int AlternativeId { get; set; }
        [Column("alternative_name")]
        public string AlternativeName { get; set; }
    }
    public class Criterion
    {
        [Key]
        [Column("criterion_id")]
        public int CriterionId { get; set; }
        [Column("criterion_name")]
        public string CriterionName { get; set; }
    }
    public class Vector
    {
        [Key]
        [Column("vector_id")]
        public int VectorId { get; set; }
        [Column("alternative_id")]
        public int AlternativeId { get; set; }
        [Column("criterion_id")]
        public int CriterionId { get; set; }
        [Column("mark")]
        public string Mark { get; set; }
        public Alternative Alternative { get; set; }
        public Criterion Criterion { get; set; }
    }
    public class LPR
    {
        [Key]
        [Column("LPR_id")]
        public int LPRId { get; set; }
        [Column("LPR_name")]
        public string LPRName { get; set; }
        [Column("LPR_range")]
        public int LPRRange { get; set; }
    }
    public class Result
    {
        [Key]
        [Column("result_id")]
        public int ResultId { get; set; }
        
        [Column("LPR_id")]
        public int LPRId { get; set; }
        [Column("LPR_range")]
        public int LPRRange { get; set; }
        [Column("alternative_id")]
        public int AlternativeId { get; set; }
        [Column("alternative_range")]
        public int AlternativeRange { get; set; }
        public Alternative Alternative { get; set; }
        public LPR LPR { get; set; }
    }
}

