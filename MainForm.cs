//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Tag_Name
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		const string nameCounter = "Кол-во ходов: ";
		const string nameTimer = "Время: ";
		
		Label CountLabel;
		Label TimerLabel;
		
		int score = 1;
		const string name = "Кол-во ходов: ";
		
		Point[,] arr;
		
		List<int> ind;
		Dictionary <int,Point> ExistingBox;
		Dictionary <int,int> HashCode_OfBox;
		
		int _Height, _Widht, _SizeOfPix;
		
		int n;
		
		public MainForm()
		{
			InitializeComponent();
			_GenMap();
		}
		void _GenMap()
		{
			n = 4;
			
			arr = new Point[n, n];
			
			_SizeOfPix = 50;
			
			_Widht = n * _SizeOfPix;
			_Height = n * _SizeOfPix;
			
			ClientSize = new Size((n + 1) * _SizeOfPix, (n) * _SizeOfPix + 1);
			
			MaximumSize = new Size(Width, Height);
			MinimumSize = new Size(Width, Height);
			
			//сетка
			for (int i = 0; i <= _Height / _SizeOfPix; i++) {
				PictureBox field = new PictureBox();
				field.Size = new Size(_Widht, 1);
				field.Location = new Point(0, _SizeOfPix * i);
				field.BackColor = Color.Gray;
				Controls.Add(field);
			}
			for (int i = 0; i <= _Widht / _SizeOfPix; i++) {
				PictureBox field = new PictureBox();
				field.Size = new Size(1, _Height);
				field.Location = new Point(_SizeOfPix * i, 0);
				field.BackColor = Color.Gray;
				Controls.Add(field);
			}
			
			//ХэшКод всех пикчюрбоксов (нужно чтобы вычислять на какой нажали) (<Код><Номер>)
			HashCode_OfBox = new Dictionary<int,int>();
			//Координаты всех пикчюрБоксов (<Номер><Координаты>)
			ExistingBox = new Dictionary<int, Point>();
			var ExistingBox2 = new Dictionary<int, Point>();
			
			for (int p = 1, i = 0; i < n; i++) {
				for (int j = 0; j < n; p++, j++) {
					
					if (p == 16) {
						PictureBox temp = new PictureBox();
						temp.Size = new Size(_SizeOfPix, _SizeOfPix);
						temp.Location = new Point(j * _SizeOfPix, i * _SizeOfPix);
					
						temp.BackColor = Color.Transparent;
						
						Controls.Add(temp);
						ExistingBox.Add(Controls.Count - 1, new Point(i * _SizeOfPix, j * _SizeOfPix));
						HashCode_OfBox.Add(temp.GetHashCode(), Controls.Count - 1);
						
						break;
					}
					
					PictureBox field = new PictureBox();
					
					field.Size = new Size(_SizeOfPix, _SizeOfPix);
					field.Location = new Point(j * _SizeOfPix, i * _SizeOfPix);
					
					field.BackColor = Color.DeepSkyBlue;
					field.Image = (Image)new Bitmap(field.Width, field.Height);
					Graphics g = Graphics.FromImage(field.Image);
					g.DrawString("" + p, new Font("Snap ITC", 10), new SolidBrush(Color.Black), new Point(10, 10));
					g.Dispose();
					
					field.Click += (_MyClick);
					Controls.Add(field);
					
					arr[i, j] = new Point(j * _SizeOfPix, i * _SizeOfPix);
					
					ExistingBox.Add(Controls.Count - 1, new Point(i * _SizeOfPix, j * _SizeOfPix));
					HashCode_OfBox.Add(field.GetHashCode(), Controls.Count - 1);
				}
			}
			
			TimerLabel = new Label();
			TimerLabel.Size = new Size(_SizeOfPix, _SizeOfPix);
			TimerLabel.Location = new Point(_SizeOfPix * n + 2, 10);
			TimerLabel.Text = "Время";
			Controls.Add(TimerLabel);
			
			CountLabel = new Label();
			CountLabel.Size = new Size(_SizeOfPix, _SizeOfPix);
			CountLabel.Font= new Font("Arial",7);
			CountLabel.Location = new Point(_SizeOfPix * n + 2, 10 + _SizeOfPix);
			CountLabel.Text = "Кол-во ходов: 0";
			Controls.Add(CountLabel);
		}
		
		void _MyClick(object sender, EventArgs e)
		{
			//индекс пикчюрбокса, который был нажат
			int indexOfPictureBox = _WhichPictureBoxWasClicked(sender.GetHashCode());
			
			if (_CanMove(indexOfPictureBox)) {
				
				int tempIndex= _indexOfEmptyPictureBox();
				Point temp = Controls[tempIndex].Location;
				
				Controls[tempIndex].Location = Controls[indexOfPictureBox].Location;
				
				Controls[indexOfPictureBox].Location = temp;
				
				CountLabel.Text=nameCounter+ score++;
			}
			
		}

		bool _CanMove(int indexOfPictureBox)
		{
			Point temp = Controls[_indexOfEmptyPictureBox()].Location;
			Point[] pointsNearPictureBox = {
				new Point(temp.X, temp.Y),
				new Point(temp.X + _SizeOfPix, temp.Y),
				new Point(temp.X - _SizeOfPix, temp.Y),
				new Point(temp.X, temp.Y + _SizeOfPix),
				new Point(temp.X, temp.Y - _SizeOfPix),
			};
			foreach (var el in pointsNearPictureBox) {
				if (el == Controls[indexOfPictureBox].Location) {
					return true;
				}
			}
			
			return false;
		}
		int _indexOfEmptyPictureBox()
		{
			foreach (var el in ExistingBox) {
				if (Controls[el.Key].BackColor == Color.Transparent) {
					return el.Key;
				}
			}
			
			return 0;
		}
		
		
		//		static int [] Shuffle(int[] arr)
		//		{
		//			Random rand = new Random();
		//			for (int i = arr.Length - 1; i >= 1; i--) {
		//				int j = rand.Next(i + 1);
		//
		//				int tmp = arr[j];
		//				arr[j] = arr[i];
		//				arr[i] = tmp;
		//			}
		//			return arr;
		//		}

		int _WhichPictureBoxWasClicked(int i)
		{
			foreach (var el in HashCode_OfBox) {
				if (i == el.Key)
					return el.Value;
			}
			return 0;
		}
		
		void MainFormKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}
		void TimerTick(object sender, EventArgs e)
		{
			//сделать таймер
		}
	}
}
