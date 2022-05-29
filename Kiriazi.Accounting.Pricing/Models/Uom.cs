using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class Uom : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged

        [Timestamp]
        public byte[] Timestamp { get; set; }

        private string _code;
        private string _name;
        
        [Required(AllowEmptyStrings = false), MaxLength(4)]
        public string Code 
        { 
            get => _code;
            set
            {
                if(_code!=value)
                {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                }
            } 
        }

        [Required(AllowEmptyStrings = false), MaxLength(250)]
        public string Name 
        { 
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public virtual ICollection<Item> Items { get; set; } = new HashSet<Item>();

        [NotMapped]
        public Uom Self => this;
    }

}
