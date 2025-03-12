using MahApps.Metro.Controls;
using SocialHelp.Models;
using System.Windows;

namespace SocialHelp
{
    public partial class CommissionEditWindow : MetroWindow
    {
        private readonly CommissionViewModel _commission;

        public CommissionEditWindow(CommissionViewModel commission)
        {
            InitializeComponent();
            _commission = commission;

            if (_commission != null)
            {
                Title = "Редактирование комиссии";
                txtGuardianshipEmployee.Text = _commission.GuardianshipEmployee != "Не указан" ? _commission.GuardianshipEmployee : "";
                txtPDNEmployee.Text = _commission.PDNEmployee != "Не указан" ? _commission.PDNEmployee : "";
                txtKDNEmployee.Text = _commission.KDNEmployee != "Не указан" ? _commission.KDNEmployee : "";
            }
            else
            {
                Title = "Добавление комиссии";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new SocialHelpContext())
            {
                Commission commissionEntity;
                if (_commission == null) // Добавление
                {
                    commissionEntity = new Commission
                    {
                        GuardianshipEmployee = string.IsNullOrWhiteSpace(txtGuardianshipEmployee.Text) ? null : txtGuardianshipEmployee.Text,
                        PDNEmployee = string.IsNullOrWhiteSpace(txtPDNEmployee.Text) ? null : txtPDNEmployee.Text,
                        KDNEmployee = string.IsNullOrWhiteSpace(txtKDNEmployee.Text) ? null : txtKDNEmployee.Text
                    };
                    context.Commissions.Add(commissionEntity);
                }
                else // Редактирование
                {
                    commissionEntity = context.Commissions.Find(_commission.Id);
                    if (commissionEntity != null)
                    {
                        commissionEntity.GuardianshipEmployee = string.IsNullOrWhiteSpace(txtGuardianshipEmployee.Text) ? null : txtGuardianshipEmployee.Text;
                        commissionEntity.PDNEmployee = string.IsNullOrWhiteSpace(txtPDNEmployee.Text) ? null : txtPDNEmployee.Text;
                        commissionEntity.KDNEmployee = string.IsNullOrWhiteSpace(txtKDNEmployee.Text) ? null : txtKDNEmployee.Text;
                    }
                }
                context.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}