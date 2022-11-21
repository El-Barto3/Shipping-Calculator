using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace grzesiowyKalkulator
{
    public partial class calculatorForm : Form
    {
        public bool glupiWybor = true, glupiWybor2=true;
        public double emptyCourseViatollRate = 0;
        public double paidCourseViatollRate = 0;

        private double combustionEmpty, fuelPricePerLitre, utilizationCostPerKm, constantCost, driverPercentage, KraKatPrice, viatollCostPerKm;
        public calculatorForm()
        {
            InitializeComponent();
        }

        private void reloadFuel()
        {
            try
            {
                overridePropertiesVariable();
                emptyFuelPerKilometerTextBox.Text = (Math.Round(combustionEmpty * fuelPricePerLitre / 100, 2)).ToString();

                paidFuelPerKilometerTextBox.Text = (Math.Round((combustionEmpty + double.Parse(założenieŁadunek.Text) * 0.3) * fuelPricePerLitre / 100, 2)).ToString();
            }
            catch { }
            
        }

        private void changeTextOfInfos(double fuelCost, double utilization, double viatollCost)
        {
            infoAboutEmptyCourseCostLabel.Text = string.Format("paliwo: {0}\neksploatacja: {1}\nviaTOLL: {2}", 
                Math.Round(fuelCost, 2).ToString("0.00"), Math.Round(utilization, 2).ToString("0.00"),Math.Round(viatollCost, 2).ToString("0.00"));
        }
        private void changeTextOfInfos2(double fuelCost, double utilization, double viatollCost, double KraKat)
        {
            infoAboutPaidCourseCostLabel.Text = string.Format("paliwo: {0}\neksploatacja: {1}\nviaTOLL: {2}\nKra-Kat: {3}",
                Math.Round(fuelCost, 2).ToString("0.00"), Math.Round(utilization, 2).ToString("0.00"), Math.Round(viatollCost, 2).ToString("0.00"), Math.Round(KraKat, 2).ToString("0.00"));
        }

        private void calculateFunction()
        {
            reloadFuel();
            try
            {
                //uploads data from textboxes to globals
                overridePropertiesVariable();

                //input data
                double emptyKM = double.Parse(emptyKMCourseTextBox.Text);
                double ładowneKM = double.Parse(daneLiczbaKilometrów.Text);

                double fracht = double.Parse(daneFrachtNetto.Text);

                double viatollZaKilometr = double.Parse(viatollCostPerKmTextBox.Text) / 100;

                //viatoll costs
                updateViatollRate();
                double emptyCourseViatollCost = viatollCostPerKm * emptyKM * emptyCourseViatollRate;
                double paidCourseViatollCost = viatollCostPerKm * ładowneKM * paidCourseViatollRate;

                //utilization cost
                double emptyUtilizationCost = utilizationCostPerKm * emptyKM;
                double paidUtilizationCost = utilizationCostPerKm * ładowneKM;

                //driver cost
                double driverCost = fracht * driverPercentage / 100;

                //drogi przejazd za ten odcinek autostrady
                double KraKatCost = 0;
                if (KatowiceCheckBox.Checked)
                    KraKatCost = KraKatPrice;

                //obliczanie spalania, zużycia itd.
                double spalanie = (combustionEmpty + double.Parse(założenieŁadunek.Text) * 0.3);

                //sumarize all labels data
                double emptyFuelCost = emptyKM * combustionEmpty / 100 * fuelPricePerLitre;
                double emptyCourseCost = emptyFuelCost + emptyCourseViatollCost + emptyUtilizationCost;
                double paidFuelCost = ładowneKM * spalanie / 100 * fuelPricePerLitre;
                double paidCourseCost = paidFuelCost + paidCourseViatollCost + paidUtilizationCost + KraKatCost;

                //sum of all costs
                double costSum = emptyCourseCost + paidCourseCost + driverCost + constantCost;
                double netto = fracht - costSum;
                double frachtPerKM = fracht / ładowneKM;

                //upload data on screen to labels
                emptyCourseCostLabel.Text = Math.Round(emptyCourseCost, 2).ToString("0.00");
                paidCourseCostLabel.Text = Math.Round(paidCourseCost, 2).ToString("0.00");
                driverCostLabel.Text = Math.Round(driverCost, 2).ToString("0.00");
                constantCostLabel.Text = Math.Round(constantCost, 2).ToString("0.00");
                totalCostLabel.Text = Math.Round(costSum, 2).ToString("0.00");
                NETTOLabel.Text = Math.Round(netto, 2).ToString("0.00");
                frachtPerKMLabel.Text = Math.Round(frachtPerKM, 2).ToString("0.00");

                backgroundColorChange(netto);
                changeTextOfInfos(emptyFuelCost, emptyUtilizationCost, emptyCourseViatollCost);
                changeTextOfInfos2(paidFuelCost, paidUtilizationCost, paidCourseViatollCost, KraKatCost);
            }
            catch
            {
                MessageBox.Show("Niepoprawne wartości");
            }
        }
        private void overridePropertiesVariable()
        {
            try
            {
                combustionEmpty = double.Parse(combustionEmptyTextBox.Text);
                fuelPricePerLitre = double.Parse(fuelPricePerLitreNettoTextBox.Text);
                utilizationCostPerKm = double.Parse(utilizationCostTextBox.Text);
                constantCost = double.Parse(constantCostTextBox.Text);
                driverPercentage = double.Parse(driverPercentageTextBox.Text);
                KraKatPrice = double.Parse(KraKatTextBox.Text);
                viatollCostPerKm = double.Parse(viatollCostPerKmTextBox.Text);
            }
            catch
            {
                //MessageBox.Show("Niepoprawne wartości założeń");
            }
        }

        private void reloadProperties()
        {
            combustionEmptyTextBox.Text = combustionEmpty.ToString();
            fuelPricePerLitreNettoTextBox.Text = fuelPricePerLitre.ToString();
            calculateBruttoNetto();
            utilizationCostTextBox.Text = utilizationCostPerKm.ToString();
            constantCostTextBox.Text = constantCost.ToString();
            driverPercentageTextBox.Text = driverPercentage.ToString();
            KraKatTextBox.Text = KraKatPrice.ToString();
            viatollCostPerKmTextBox.Text = viatollCostPerKm.ToString();

            //MessageBox.Show("Spalanie: "+ combustionEmpty+ "\nCena paliwa netto: " + fuelPricePerLitre);
            reloadFuel();

        }
        private void backgroundColorChange(double value)
        {
            if (value >= 0)
                NETTOLabel.ForeColor = Color.Green;
            else
                NETTOLabel.ForeColor = Color.Red;

        }

        private void invalidViatollCase(RadioButton c)
        {
            c.Checked = true;             
            MessageBox.Show("Niepoprawna wartość procentowa viaTOLLa");
            updateViatollRate();
        }

        private void updateViatollRate()
        {

            if (emptyOption0.Checked)
                emptyCourseViatollRate = 0;
            if (emptyOption33.Checked)
                emptyCourseViatollRate = 0.33;
            if (emptyOption66.Checked)
                emptyCourseViatollRate = 0.66;
            if (emptyOption100.Checked)
                emptyCourseViatollRate = 1;
            if(emptyOptionOther.Checked)
            {
                try
                {
                    double rate = double.Parse(emptyOptionOtherTextBox.Text);
                    if (rate >= 0 && rate <= 100)
                        emptyCourseViatollRate = rate / 100;
                    else
                        invalidViatollCase(emptyOption0);
                }
                catch
                {
                    invalidViatollCase(emptyOption0);
                }
            }
            //MessageBox.Show("VIATOL pusty: " + emptyCourseViatollRate.ToString());

            if (paidOption0.Checked)
                paidCourseViatollRate = 0;
            if (paidOption33.Checked)
                paidCourseViatollRate = 0.33;
            if (paidOption66.Checked)
                paidCourseViatollRate = 0.66;
            if (paidOption100.Checked)
                paidCourseViatollRate = 1;
            if (paidOptionOther.Checked)
            {
                try
                {
                    double rate = double.Parse(paidOptionOtherTextBox.Text);
                    if (rate >= 0 && rate <= 100)
                        paidCourseViatollRate = rate / 100;
                    else
                        invalidViatollCase(paidOption0);
                }
                catch
                {
                    invalidViatollCase(paidOption0);
                }
            }
            //MessageBox.Show("VIATOL paid: " + paidCourseViatollRate.ToString());
        }
        

        

        private void potwierdźDanePrzycisk_Click(object sender, EventArgs e)
        {
            calculateFunction();
        }

        private void ZaWarudo_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void safeConfigButton_Click(object sender, EventArgs e)
        {
            try
            {
                overridePropertiesVariable();
                Properties.Settings.Default.combustion = combustionEmpty;
                Properties.Settings.Default.fuelPrice = fuelPricePerLitre;
                Properties.Settings.Default.utilization = utilizationCostPerKm;
                Properties.Settings.Default.constant = constantCost;
                Properties.Settings.Default.driver = driverPercentage;
                Properties.Settings.Default.Krakat = KraKatPrice;
                Properties.Settings.Default.viatoll = viatollCostPerKm;
                Properties.Settings.Default.Save();
            }
            catch
            {
                MessageBox.Show("Nie udało się zapisać konfiguracji");
            }
        }
        private void ViatollPrzycisk_Click(object sender, EventArgs e)
        {
                viatollGroupBox.Visible = true;
        }
        private void emptyCourseCostLabel_MouseHover(object sender, EventArgs e)
        {
            Thread.Sleep(500);
            infoAboutEmptyCourseCostLabel.Visible = true;
            emptyCourseCostLabel.BackColor = Color.White;
        }
        private void emptyCourseCostLabel_MouseLeave(object sender, EventArgs e)
        {
            Thread.Sleep(500);
            infoAboutEmptyCourseCostLabel.Visible = false;
            emptyCourseCostLabel.BackColor = Color.LightGray;
        }

        private void paidCourseCostLabel_MouseHover(object sender, EventArgs e)
        {
            Thread.Sleep(500);
            infoAboutPaidCourseCostLabel.Visible = true;
            paidCourseCostLabel.BackColor = Color.White;
        }

        private void paidCourseCostLabel_MouseLeave(object sender, EventArgs e)
        {
            Thread.Sleep(500);
            infoAboutPaidCourseCostLabel.Visible = false;
            paidCourseCostLabel.BackColor = Color.LightGray;
        }

        private void założenieŁadunek_TextChanged(object sender, EventArgs e)
        {
            reloadFuel();
        }

        private void combustionEmptyTextBox_TextChanged(object sender, EventArgs e)
        {
            reloadFuel();
        }

        // ------------- netto / brutto ------------------

        private void calculateBruttoNetto()
        {
            try
            {
                double netto = double.Parse(fuelPricePerLitreNettoTextBox.Text);

                double brutto = netto * 123 / 100;

                fuelPricePerLitreBruttoTextbox.Text = Math.Round(brutto, 2).ToString();

                reloadFuel();
            }
            catch
            {
                MessageBox.Show("Niepoprawne dane paliwowe");
            }         
        }
        private void założenieCenaPaliwa_TextChanged(object sender, EventArgs e)
        {
            calculateBruttoNetto();
        }

        private void KraKatTextbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double price = double.Parse(KraKatTextBox.Text);
                KraKatPrice = price;
            }
            catch
            {
                MessageBox.Show("Niepoprawne dane");
            }
        }

        private void General_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.GetNextControl(ActiveControl, true) != null)
                {                 
                    e.Handled = true;
                    this.GetNextControl(ActiveControl, true).Focus();
                }
            }
        }

        private void calculatorForm_Load(object sender, EventArgs e)
        {
            combustionEmpty = Properties.Settings.Default.combustion;
            fuelPricePerLitre = Properties.Settings.Default.fuelPrice;
            utilizationCostPerKm = Properties.Settings.Default.utilization;
            constantCost = Properties.Settings.Default.constant;
            driverPercentage = Properties.Settings.Default.driver;
            KraKatPrice = Properties.Settings.Default.Krakat;
            viatollCostPerKm = Properties.Settings.Default.viatoll;
            reloadProperties();
        }

        private void recalculateButton_Click(object sender, EventArgs e)
        {
            calculateFunction();
        }

        private void założenieCenaPaliwaBrutto_TextChanged(object sender, EventArgs e)
        {
            if (fuelPricePerLitreBruttoTextbox.ContainsFocus)
            {

                try
                {
                    double brutto = double.Parse(fuelPricePerLitreBruttoTextbox.Text);
                    double netto = brutto * 100 / 123;

                    fuelPricePerLitreNettoTextBox.Text = Math.Round(netto, 2).ToString();

                }
                catch
                {
                    MessageBox.Show("Niepoprawne dane");
                }
            }
        }
    }
}
