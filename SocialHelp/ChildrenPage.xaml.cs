using SocialHelp.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SocialHelp
{
    public partial class ChildrenPage : Page
    {
        private SocialHelpContext _context;

        public ChildrenPage()
        {
            InitializeComponent();
            _context = new SocialHelpContext();
            LoadChildren();
        }

        private void LoadChildren(string filter = "")
        {
            var children = _context.Children
                .Where(c => string.IsNullOrEmpty(filter) || c.FullName.Contains(filter))
                .Select(c => new ChildViewModel
                {
                    Id = c.ChildId,
                    FullName = c.FullName,
                    BirthDate = c.BirthDate,
                    Gender = c.Gender,
                    HasDisability = c.HasDisability,
                    School = c.School,
                    Notes = c.Notes
                })
                .ToList();

            childrenList.ItemsSource = children;
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadChildren(txtFilter.Text);
        }

        private void btnAddChild_Click(object sender, RoutedEventArgs e)
        {
            ChildEditWindow editWindow = new ChildEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadChildren(txtFilter.Text);
            }
        }
    }

    public class ChildViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public bool HasDisability { get; set; }
        public string School { get; set; }
        public string Notes { get; set; }
    }
}