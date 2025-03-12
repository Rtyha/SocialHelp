using SocialHelp.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SocialHelp
{
    public partial class FamiliesPage : Page
    {
        private SocialHelpContext _context;

        public FamiliesPage()
        {
            InitializeComponent();
            _context = new SocialHelpContext();
            LoadFamilies();
        }

        public void LoadFamilies(string filter = "")
        {
            var families = _context.Families
                .Where(f => string.IsNullOrEmpty(filter) || f.FamilyName.Contains(filter))
                .Select(f => new FamilyViewModel
                {
                    Id = f.FamilyId,
                    FamilyName = f.FamilyName,
                    FatherName = f.Father != null ? f.Father.FullName : "Не указан",
                    MotherName = f.Mother != null ? f.Mother.FullName : "Не указан",
                    Status = f.Status,
                    ChildrenCount = f.ChildrenInFamilies.Count // Теперь должно работать
                })
                .ToList();

            familiesList.ItemsSource = families;
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadFamilies(txtFilter.Text);
        }

        private void btnAddFamily_Click(object sender, RoutedEventArgs e)
        {
            FamilyEditWindow editWindow = new FamilyEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadFamilies(txtFilter.Text);
            }
        }
    }

    public class FamilyViewModel
    {
        public int Id { get; set; }
        public string FamilyName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string Status { get; set; }
        public int ChildrenCount { get; set; }
    }
}