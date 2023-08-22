using StudioManagement.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace StudioManagement.Data
{
    public class StudioRegister
    {
        [Key]
        [DisplayName("Register ID")]
        public int RegisterId { get; set; }

        [DisplayName("Studio ID")]
        public int StudioID { get; set; }

        [DisplayName("Member ID")]
        public int MemberId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        [ForeignKey("StudioID")]
        [ValidateNever]
        public Studio Studio { get; set; }

        [ForeignKey("MemberId")]
        [ValidateNever]
        public Member Member { get; set; }
    }
}
