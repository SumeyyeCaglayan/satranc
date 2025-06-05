namespace chess {
	public partial class Form1 : Form {
		public static Button[,] butonlar = new Button[8, 8];
		public Cell[,] Board = chess.Program.ches.Board;
		public static Cell? selected_cell;
		public Color board_color_0 = Color.White; //Color.Beige;
		public Color board_color_1 = Color.LightGray; //Color.BurlyWood;
		public Color piece_color_0 = Color.Brown;
		public Color piece_color_1 = Color.BlueViolet;


		public Form1() {
			InitializeComponent();
			Board = chess.Program.ches.Board;
			butonlarý_oluþtur();
			update();
		}
		void update() {
			color_board();
			print_board();
		}

		void butonlarý_oluþtur() {
			int buton_size_x = panel1.Width / 8;
			int buton_size_y = panel1.Height / 8;

			for(int i = 0; i < 8; i++) {
				for(int j = 0; j < 8; j++) {
					Button buton = new Button();
					butonlar[i, j] = buton;

					buton.Height = buton_size_y;
					buton.Width = buton_size_x;

					buton.Click += buton_click;
					panel1.Controls.Add(buton);
					buton.Location = new Point(i * buton_size_x, j* buton_size_y);
					buton.Tag = new Point(i, j);
				}
			}

		}

		public void highlight_legal_moves_for(int x, int y) {
			color_board();
			ChessPiece ch = Board[x, y].occupied_by as ChessPiece;
			if(ch == null) {
				selected_cell = null;
				return;
			}

			var lm = ch.find_legal_moves();
			for(int i = 0; i < lm.Count; i++) {
				Button but = butonlar[lm[i][0], lm[i][1]];
				but.BackColor = Color.Green;
			}
			if(lm.Count == 0) {
				selected_cell = null;
			}
		}

		void print_board() {
			for(int i = 0; i < 8; i++) {
				for(int j = 0; j < 8; j++) {
					ChessPiece cp = Board[i, j].occupied_by;
					if(cp != null) {
						// Taþlarýn Yazýsý. Resim ekledim.
						//butonlar[i, j].Text = cp.GetType().Name;
						//butonlar[i, j].ForeColor = (cp.color == 0) ? piece_color_0 : piece_color_1;
						buton_resmini_ayarla(butonlar[i, j], cp);
					}
					else {
						butonlar[i, j].Text = "";
						butonlar[i, j].Image = null;
					}
				}
			}
		}
		void buton_resmini_ayarla(Button buton, ChessPiece cp) {
			string path = @"..\..\..\64px\b_pawn.png";
			switch(cp.GetType().Name) {
				case "Pawn":
					path = (cp.color == 0) ? @"..\..\..\64px\b_pawn.png" : @"..\..\..\64px\w_pawn.png";
					break;
				case "Rook":
					path = (cp.color == 0) ? @"..\..\..\64px\b_rook.png" : @"..\..\..\64px\w_rook.png";
					break;
				case "Bishop":
					path = (cp.color == 0) ? @"..\..\..\64px\b_bishop.png" : @"..\..\..\64px\w_bishop.png";
					break;
				case "Knight":
					path = (cp.color == 0) ? @"..\..\..\64px\b_knight.png" : @"..\..\..\64px\w_knight.png";
					break;
				case "King":
					path = (cp.color == 0) ? @"..\..\..\64px\b_king.png" : @"..\..\..\64px\w_king.png";
					break;
				case "Queen":
					path = (cp.color == 0) ? @"..\..\..\64px\b_queen.png" : @"..\..\..\64px\w_queen.png";
					break;
			}
			buton.Image = Image.FromFile(path);
		}
		void color_board() {
			for(int i = 0; i < 8; i++) {
				for(int j = 0; j < 8; j++) {
					Button buton = butonlar[i, j];

					if(i % 2 == 0){
						if(j % 2 == 0) {
							buton.BackColor = board_color_0;
						}
						else {
							buton.BackColor = board_color_1;
						}
					}
					else{
						if(j % 2 == 0) {
							buton.BackColor = board_color_1;
						}
						else {
							buton.BackColor = board_color_0;
						}
					}
				}
			}
		}


		private void buton_click(object? sender, EventArgs e) {
			Button buton = (Button)sender;
			Point point = (Point)buton.Tag;

			if(selected_cell != null && selected_cell.occupied_by != null) {
				if(sahip(selected_cell.occupied_by.legal_moves, point.X, point.Y)) {
					chess.Program.ches.move_piece(selected_cell, point.X, point.Y);
					update();
					selected_cell = null;
				}
				else {
					color_board();
					highlight_legal_moves_for(point.X, point.Y);
					selected_cell = Board[point.X,point.Y];
				}
				
			}
			else {
				var _cell = Board[point.X, point.Y];
				if(_cell.occupied_by != null) {
					selected_cell = _cell;
				}
				highlight_legal_moves_for(point.X, point.Y);
			}

		}

		bool sahip(List<int[]> li, int x, int y) {
			for(int i = 0; i < li.Count; i++) {
				if(li[i][0] == x && li[i][1] == y) {
					return true;
				}
			}
			return false;
		}

	}
}