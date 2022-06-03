using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class Group : INotifyPropertyChanged
    {
        private string _name;
        private string _description;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get;  set; } = Guid.NewGuid();
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required(AllowEmptyStrings = false), MaxLength(250)]
        public string Name 
        { 
            get=>_name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        [MaxLength(500)]
        public string Description 
        { 
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public virtual ICollection<CompanyItemAssignment> ItemAssignments { get; set; } = new HashSet<CompanyItemAssignment>();

        [NotMapped]

        public Group Self => this;
    }
}
