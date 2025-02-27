using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class TaskDetails
    {
        [Key]
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string? TaskDescription { get; set; }
        public int TaskAssignedToUserId { get; set; }
        public string TaskStatus { get; set; } //Not-Started,In-Progress,Completed
        public int TaskSize { get; set; }
    }
}
