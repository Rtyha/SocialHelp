using SocialHelp;
using SocialHelp.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SocialHelp.Pages
{
    public partial class ParentsPage : Page
    {
        private SocialHelpContext _context;

        public ParentsPage()
        {
            InitializeComponent();
            _context = new SocialHelpContext();
            LoadParents();
        }

        private void LoadParents(string filter = "")
        {
            var mothers = _context.Mothers
                .Where(m => string.IsNullOrEmpty(filter) || m.FullName.Contains(filter))
                .Select(m => new ParentViewModel
                {
                    Id = m.MotherId,
                    FullName = m.FullName,
                    Type = "Мать",
                    Address = m.Address,
                    BirthDate = m.BirthDate,
                    PhoneNumber = m.PhoneNumber
                })
                .ToList();

            var fathers = _context.Fathers
                .Where(f => string.IsNullOrEmpty(filter) || f.FullName.Contains(filter))
                .Select(f => new ParentViewModel
                {
                    Id = f.FatherId,
                    FullName = f.FullName,
                    Type = "Отец",
                    Address = f.Address,
                    BirthDate = f.BirthDate,
                    PhoneNumber = f.PhoneNumber
                })
                .ToList();

            parentsList.ItemsSource = mothers.Concat(fathers).ToList();
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadParents(txtFilter.Text);
        }

        private void btnAddParent_Click(object sender, RoutedEventArgs e)
        {
            ParentEditWindow editWindow = new ParentEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadParents(txtFilter.Text);
            }
        }
    }

    public class ParentViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public string PhoneNumber { get; set; }
    }
}