using System;
using System.Collections.Generic;
using System.Linq;
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

namespace TransMan
{
    /// <summary>
    /// Interaction logic for UpdateDialog.xaml
    /// </summary>
    public partial class UpdateDialog : Window
    {
        private TranslationItem ti;

        public TranslationItem TranslationItem
        {
            get { return ti; }
            set
            {
                ti = value;
                tbKeywordOld.Text = ti.Keyword;
                tbEnglishOld.Text = ti.English;
                tbGermanOld.Text = ti.German;
            }
        }

        public UpdateDialog()
        {
            InitializeComponent();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbEnglishNew.Text) || string.IsNullOrWhiteSpace(tbGermanNew.Text) || string.IsNullOrWhiteSpace(tbKeywordNew.Text))
            {
                MessageBox.Show("All fields must be filled!!!");
                return;
            }

            TranslationItem.UpdatedItem = new TranslationItem(tbKeywordNew.Text, tbEnglishNew.Text, tbGermanNew.Text, TranslationItem.OperationTypeEnum.Update);
            
            this.DialogResult = true;
            this.Close();
        }
    }
}
