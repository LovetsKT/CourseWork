using CoinCollectionApp.Services;
using Couresework2.CoinCollectionApp.Models;

namespace CoinCollectionApp.Forms
{
    public class MainForm : Form
    {
        private readonly DataService _dataService;
        private CollectionData _collectionData;
        private DictionaryData _dictionaryData;

        private DataGridView _gridCoins;
        private TextBox _txtSearch;
        private ComboBox _cmbFilterStatus;
        private Label _lblStats;
        private Button _btnAdd;
        private Button _btnEdit;
        private Button _btnDelete;

        public MainForm()
        {
            _dataService = new DataService();
            _collectionData = _dataService.LoadCollection();
            _dictionaryData = _dataService.LoadDictionaries();

            InitializeUI();
            RefreshGrid();
        }

        private void InitializeUI()
        {
            this.Text = "Система управления коллекцией монет";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };

            Label lblSearch = new Label { Text = "Поиск:", AutoSize = true, Location = new Point(10, 15) };
            _txtSearch = new TextBox { Location = new Point(60, 12), Width = 200 };
            _txtSearch.TextChanged += (s, e) => RefreshGrid();

            Label lblStatus = new Label { Text = "Раздел:", AutoSize = true, Location = new Point(280, 15) };
            _cmbFilterStatus = new ComboBox { Location = new Point(335, 12), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbFilterStatus.Items.Add("Все монеты");
            _cmbFilterStatus.Items.Add("В коллекции");
            _cmbFilterStatus.Items.Add("Список желаемого");
            _cmbFilterStatus.Items.Add("На обмен");
            _cmbFilterStatus.SelectedIndex = 0;
            _cmbFilterStatus.SelectedIndexChanged += (s, e) => RefreshGrid();

            _btnAdd = new Button { Text = "+ Добавить монету", Location = new Point(520, 10), Width = 140, Height = 28, BackColor = Color.LightGreen };
            _btnAdd.Click += BtnAdd_Click;

            _btnEdit = new Button { Text = "Редактировать", Location = new Point(670, 10), Width = 120, Height = 28 };
            _btnEdit.Click += BtnEdit_Click;

            _btnDelete = new Button { Text = "Удалить", Location = new Point(800, 10), Width = 100, Height = 28, BackColor = Color.LightCoral };
            _btnDelete.Click += BtnDelete_Click;

            topPanel.Controls.AddRange(new Control[] { lblSearch, _txtSearch, lblStatus, _cmbFilterStatus, _btnAdd, _btnEdit, _btnDelete });

            Panel bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = Color.WhiteSmoke, Padding = new Padding(10) };
            _lblStats = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            bottomPanel.Controls.Add(_lblStats);

            _gridCoins = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            this.Controls.Add(_gridCoins);
            this.Controls.Add(topPanel);
            this.Controls.Add(bottomPanel);
        }

        private void RefreshGrid()
        {
            string search = _txtSearch.Text.Trim().ToLower();

            var filteredCoins = _collectionData.Coins.Where(c =>
                (string.IsNullOrEmpty(search) || c.Name.ToLower().Contains(search) || c.Year.ToString().Contains(search)) &&
                (_cmbFilterStatus.SelectedIndex == 0 ||
                 (_cmbFilterStatus.SelectedIndex == 1 && c.Status == CoinStatus.InCollection) ||
                 (_cmbFilterStatus.SelectedIndex == 2 && c.Status == CoinStatus.Wishlist) ||
                 (_cmbFilterStatus.SelectedIndex == 3 && c.Status == CoinStatus.ForExchange))
            );

            var displayList = filteredCoins.Select(c => new
            {
                Id = c.Id,
                Название = c.Name,
                Номинал = $"{c.FaceValue:G} {c.Currency}",
                Год = c.Year,
                Страна = _dictionaryData.Countries.FirstOrDefault(x => x.Id == c.CountryId)?.Name ?? "Не указана",
                Металл = _dictionaryData.Metals.FirstOrDefault(x => x.Id == c.MetalId)?.Name ?? "Не указан",
                Состояние = _dictionaryData.Conditions.FirstOrDefault(x => x.Id == c.ConditionId)?.Name ?? "-",
                Покупка = $"{c.PurchasePrice:N2} ₽",
                Оценка = $"{c.MarketValue:N2} ₽",
                Фото = c.ImagePaths.Count > 0 ? $"Есть ({c.ImagePaths.Count})" : "Нет"
            }).ToList();

            _gridCoins.DataSource = displayList;
            if (_gridCoins.Columns["Id"] != null) _gridCoins.Columns["Id"].Visible = false;

            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            var myCoins = _collectionData.Coins.Where(c => c.Status == CoinStatus.InCollection).ToList();
            decimal totalSpent = myCoins.Sum(c => c.PurchasePrice);
            decimal totalMarket = myCoins.Sum(c => c.MarketValue);
            decimal profit = totalMarket - totalSpent;

            _lblStats.Text = $"В коллекции: {myCoins.Count} шт.  |  Затрачено: {totalSpent:N2} ₽  |  Рыночная стоимость: {totalMarket:N2} ₽  |  Прибыль: {profit:+#,##0.00;-#,##0.00;0.00} ₽";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var editForm = new CoinEditForm(_dictionaryData, _dataService);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                _collectionData.Coins.Add(editForm.ResultCoin);
                _dataService.SaveCollection(_collectionData);
                RefreshGrid();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_gridCoins.SelectedRows.Count == 0) return;

            Guid selectedId = (Guid)_gridCoins.SelectedRows[0].Cells["Id"].Value;
            var coinToEdit = _collectionData.Coins.FirstOrDefault(c => c.Id == selectedId);

            if (coinToEdit != null)
            {
                var editForm = new CoinEditForm(_dictionaryData, _dataService, coinToEdit);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    _dataService.SaveCollection(_collectionData);
                    RefreshGrid();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_gridCoins.SelectedRows.Count == 0) return;

            if (MessageBox.Show("Удалить выбранную монету и все её фотографии?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Guid selectedId = (Guid)_gridCoins.SelectedRows[0].Cells["Id"].Value;
                var coinToDelete = _collectionData.Coins.FirstOrDefault(c => c.Id == selectedId);

                if (coinToDelete != null)
                {
                    foreach (var imgPath in coinToDelete.ImagePaths)
                    {
                        _dataService.DeleteSingleImage(imgPath);
                    }

                    _collectionData.Coins.Remove(coinToDelete);
                    _dataService.SaveCollection(_collectionData);
                    RefreshGrid();
                }
            }
        }
    }
}
