using CoinCollectionApp.Services;
using Couresework2.CoinCollectionApp.Models;

namespace CoinCollectionApp.Forms
{
    public class CoinEditForm : Form
    {
        #region Свойства и поля

        public Coin ResultCoin { get; private set; }

        private readonly DictionaryData _dicts;
        private readonly DataService _dataService;

        private List<string> _tempImagePaths;
        private int _currentPhotoIndex = -1;

        private TextBox _txtName;
        private NumericUpDown _numFaceValue;
        private TextBox _txtCurrency;
        private NumericUpDown _numYear;
        private ComboBox _cmbCountry;
        private ComboBox _cmbMetal;
        private ComboBox _cmbCondition;
        private ComboBox _cmbEdge;
        private NumericUpDown _numPurchasePrice;
        private NumericUpDown _numMarketValue;
        private ComboBox _cmbStatus;

        private PictureBox _picCoin;
        private Label _lblPhotoCounter;
        private Button _btnPrevPhoto;
        private Button _btnNextPhoto;
        private Button _btnAddPhoto;
        private Button _btnRemovePhoto;

        #endregion

        #region Конструктор

        public CoinEditForm(DictionaryData dicts, DataService dataService, Coin existingCoin = null)
        {
            _dicts = dicts;
            _dataService = dataService;

            ResultCoin = existingCoin ?? new Coin { Year = DateTime.Now.Year };

            _tempImagePaths = new List<string>(ResultCoin.ImagePaths);

            InitializeUI();
            LoadCoinDataToForm();
        }

        #endregion

        #region Построение интерфейса

        private void InitializeUI()
        {
            this.Text = "Карточка монеты";
            this.Size = new Size(730, 480);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int leftX = 20;
            int inputX = 140;
            int y = 20;
            int step = 36;

            AddLabel("Название:", leftX, y);
            _txtName = new TextBox { Location = new Point(inputX, y), Width = 220 };
            y += step;

            AddLabel("Номинал / Валюта:", leftX, y);
            _numFaceValue = new NumericUpDown
            {
                Location = new Point(inputX, y),
                Width = 100,
                DecimalPlaces = 2,
                Maximum = 1000000
            };
            _txtCurrency = new TextBox { Location = new Point(inputX + 110, y), Width = 110 };
            y += step;

            AddLabel("Год выпуска:", leftX, y);
            _numYear = new NumericUpDown
            {
                Location = new Point(inputX, y),
                Width = 100,
                Minimum = 1,
                Maximum = 2100
            };
            y += step;

            AddLabel("Страна:", leftX, y);
            _cmbCountry = new ComboBox
            {
                Location = new Point(inputX, y),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = _dicts.Countries.ToList()
            };
            y += step;

            AddLabel("Металл:", leftX, y);
            _cmbMetal = new ComboBox
            {
                Location = new Point(inputX, y),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = _dicts.Metals.ToList()
            };
            y += step;

            AddLabel("Состояние:", leftX, y);
            _cmbCondition = new ComboBox
            {
                Location = new Point(inputX, y),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = _dicts.Conditions.ToList()
            };
            y += step;

            AddLabel("Тип гурта:", leftX, y);
            _cmbEdge = new ComboBox
            {
                Location = new Point(inputX, y),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDown
            };
            _cmbEdge.Items.AddRange(new string[]
            {
                "Гладкий", "Рубчатый", "Рифленый",
                "Сетчатый", "Шнуровидный", "С надписью"
            });
            y += step;

            AddLabel("Цена покупки:", leftX, y);
            _numPurchasePrice = new NumericUpDown
            {
                Location = new Point(inputX, y),
                Width = 220,
                DecimalPlaces = 2,
                Maximum = 10000000
            };
            y += step;

            AddLabel("Рыночная цена:", leftX, y);
            _numMarketValue = new NumericUpDown
            {
                Location = new Point(inputX, y),
                Width = 220,
                DecimalPlaces = 2,
                Maximum = 10000000
            };
            y += step;

            AddLabel("Статус:", leftX, y);
            _cmbStatus = new ComboBox
            {
                Location = new Point(inputX, y),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbStatus.Items.AddRange(new string[]
            {
                "В коллекции", "Список желаемого", "На обмен"
            });

            int rightX = 400;

            _picCoin = new PictureBox
            {
                Location = new Point(rightX, 20),
                Size = new Size(290, 260),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.WhiteSmoke
            };

            _btnPrevPhoto = new Button { Text = "<", Location = new Point(rightX, 290), Width = 40 };
            _btnPrevPhoto.Click += (s, e) =>
            {
                if (_currentPhotoIndex > 0)
                {
                    _currentPhotoIndex--;
                    UpdateGalleryUI();
                }
            };

            _lblPhotoCounter = new Label
            {
                Text = "0 / 0",
                Location = new Point(rightX + 45, 295),
                Width = 200,
                TextAlign = ContentAlignment.MiddleCenter
            };

            _btnNextPhoto = new Button { Text = ">", Location = new Point(rightX + 250, 290), Width = 40 };
            _btnNextPhoto.Click += (s, e) =>
            {
                if (_currentPhotoIndex < _tempImagePaths.Count - 1)
                {
                    _currentPhotoIndex++;
                    UpdateGalleryUI();
                }
            };

            _btnAddPhoto = new Button { Text = "+ Добавить фото", Location = new Point(rightX, 325), Width = 140 };
            _btnAddPhoto.Click += BtnAddPhoto_Click;

            _btnRemovePhoto = new Button { Text = "Удалить кадр", Location = new Point(rightX + 150, 325), Width = 140 };
            _btnRemovePhoto.Click += BtnRemovePhoto_Click;

            Button btnSave = new Button
            {
                Text = "Сохранить монету",
                Location = new Point(rightX, 380),
                Width = 290,
                Height = 40,
                BackColor = Color.LightSkyBlue,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.Click += BtnSave_Click;

            this.Controls.AddRange(new Control[]
            {
                _txtName, _numFaceValue, _txtCurrency, _numYear,
                _cmbCountry, _cmbMetal, _cmbCondition, _cmbEdge,
                _numPurchasePrice, _numMarketValue, _cmbStatus,
                _picCoin, _btnPrevPhoto, _lblPhotoCounter, _btnNextPhoto,
                _btnAddPhoto, _btnRemovePhoto, btnSave
            });
        }

        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                AutoSize = true
            });
        }

        #endregion

        #region Заполнение формы данными монеты

        private void LoadCoinDataToForm()
        {
            _txtName.Text = ResultCoin.Name;
            _numFaceValue.Value = ResultCoin.FaceValue;
            _txtCurrency.Text = ResultCoin.Currency;
            _numYear.Value = ResultCoin.Year;
            _cmbEdge.Text = ResultCoin.EdgeType;
            _numPurchasePrice.Value = ResultCoin.PurchasePrice;
            _numMarketValue.Value = ResultCoin.MarketValue;

            if (ResultCoin.CountryId != Guid.Empty)
                _cmbCountry.SelectedItem = _dicts.Countries.FirstOrDefault(x => x.Id == ResultCoin.CountryId);

            if (ResultCoin.MetalId != Guid.Empty)
                _cmbMetal.SelectedItem = _dicts.Metals.FirstOrDefault(x => x.Id == ResultCoin.MetalId);

            if (ResultCoin.ConditionId != Guid.Empty)
                _cmbCondition.SelectedItem = _dicts.Conditions.FirstOrDefault(x => x.Id == ResultCoin.ConditionId);

            _cmbStatus.SelectedIndex = ResultCoin.Status == CoinStatus.Wishlist ? 1 :
                                       (ResultCoin.Status == CoinStatus.ForExchange ? 2 : 0);

            if (_tempImagePaths.Count > 0) _currentPhotoIndex = 0;
            UpdateGalleryUI();
        }

        #endregion

        #region Галерея изображений

        private void UpdateGalleryUI()
        {
            if (_picCoin.Image != null)
            {
                _picCoin.Image.Dispose();
                _picCoin.Image = null;
            }

            if (_tempImagePaths.Count == 0 || _currentPhotoIndex < 0)
            {
                _lblPhotoCounter.Text = "Нет фото";
                _btnPrevPhoto.Enabled = false;
                _btnNextPhoto.Enabled = false;
                _btnRemovePhoto.Enabled = false;
                return;
            }

            _lblPhotoCounter.Text = $"Фото {_currentPhotoIndex + 1} из {_tempImagePaths.Count}";
            _btnPrevPhoto.Enabled = _currentPhotoIndex > 0;
            _btnNextPhoto.Enabled = _currentPhotoIndex < _tempImagePaths.Count - 1;
            _btnRemovePhoto.Enabled = true;

            string fullPath = _dataService.GetAbsoluteImagePath(_tempImagePaths[_currentPhotoIndex]);
            if (File.Exists(fullPath))
            {
                using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    _picCoin.Image = Image.FromStream(fs);
                }
            }
        }

        private void BtnAddPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string savedPath = _dataService.CopyImageToStorage(ofd.FileName);
                    if (savedPath != null)
                    {
                        _tempImagePaths.Add(savedPath);
                        _currentPhotoIndex = _tempImagePaths.Count - 1;
                        UpdateGalleryUI();
                    }
                }
            }
        }

        private void BtnRemovePhoto_Click(object sender, EventArgs e)
        {
            if (_currentPhotoIndex < 0 || _currentPhotoIndex >= _tempImagePaths.Count) return;

            string pathToDelete = _tempImagePaths[_currentPhotoIndex];

            if (_picCoin.Image != null)
            {
                _picCoin.Image.Dispose();
                _picCoin.Image = null;
            }

            _dataService.DeleteSingleImage(pathToDelete);
            _tempImagePaths.RemoveAt(_currentPhotoIndex);

            if (_currentPhotoIndex >= _tempImagePaths.Count)
                _currentPhotoIndex = _tempImagePaths.Count - 1;

            UpdateGalleryUI();
        }

        #endregion

        #region Сохранение

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text))
            {
                MessageBox.Show("Введите название монеты!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ResultCoin.Name = _txtName.Text.Trim();
            ResultCoin.FaceValue = _numFaceValue.Value;
            ResultCoin.Currency = _txtCurrency.Text.Trim();
            ResultCoin.Year = (int)_numYear.Value;
            ResultCoin.EdgeType = _cmbEdge.Text.Trim();
            ResultCoin.PurchasePrice = _numPurchasePrice.Value;
            ResultCoin.MarketValue = _numMarketValue.Value;

            if (_cmbCountry.SelectedItem is Country c) ResultCoin.CountryId = c.Id;
            if (_cmbMetal.SelectedItem is Metal m) ResultCoin.MetalId = m.Id;
            if (_cmbCondition.SelectedItem is Condition cond) ResultCoin.ConditionId = cond.Id;

            ResultCoin.Status = _cmbStatus.SelectedIndex == 1 ? CoinStatus.Wishlist :
                                (_cmbStatus.SelectedIndex == 2 ? CoinStatus.ForExchange : CoinStatus.InCollection);

            ResultCoin.ImagePaths = _tempImagePaths;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion
    }
}