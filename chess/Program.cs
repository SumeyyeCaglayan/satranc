namespace chess {
	public static class Program {
		public static Chess ches = new Chess();

		[STAThread]
		static void Main() {
			ApplicationConfiguration.Initialize();
			Application.Run(new Form1());
		}
	}
	public class Chess {
		public Cell[,] Board = new Cell[8, 8];

		public Chess() {
			create_board();
			fill_board();
		}

		void create_board() {
			for(int i = 0; i < 8; i++) {
				for(int j = 0; j < 8; j++) {
					Board[i, j] = new Cell(i, j);
				}
			}
		}

		void fill_board() {
			// piyonlar
			for(int i = 0; i < 8; i++) {
				create_piece("Pawn", i, 1, 0);
			}
			for(int i = 0; i < 8; i++) {
				create_piece("Pawn", i, 6, 1);
			}

			// kaleler
			create_piece("Rook", 0, 0, 0);
			create_piece("Rook", 7, 0, 0);
			create_piece("Rook", 0, 7, 1);
			create_piece("Rook", 7, 7, 1);

			// filler
			create_piece("Bishop", 2, 0, 0);
			create_piece("Bishop", 5, 0, 0);
			create_piece("Bishop", 2, 7, 1);
			create_piece("Bishop", 5, 7, 1);

			// atlar
			create_piece("Knight", 1, 0, 0);
			create_piece("Knight", 6, 0, 0);
			create_piece("Knight", 1, 7, 1);
			create_piece("Knight", 6, 7, 1);

			// krallar
			create_piece("King", 4, 0, 0);
			create_piece("King", 4, 7, 1);

			// vezirler
			create_piece("Queen", 3, 0, 0);
			create_piece("Queen", 3, 7, 1);
		}

		void create_piece(string piece_index, int x, int y, int team) {
			Cell cel = Board[x, y];
			ChessPiece piece = new ChessPiece(9, 9, 2);
			switch(piece_index) {
				case "Pawn":
					piece = new Pawn(x, y, team);
					break;
				case "Rook":
					piece = new Rook(x, y, team);
					break;
				case "Bishop":
					piece = new Bishop(x, y, team);
					break;
				case "Knight":
					piece = new Knight(x, y, team);
					break;
				case "King":
					piece = new King(x, y, team);
					break;
				case "Queen":
					piece = new Queen(x, y, team);
					break;
				default:
					break;
			}
			cel.occupied_by = piece;
		}


		public void move_piece(Cell cel0, int x, int y) {
			move_piece(cel0, Board[x, y]);
		}
		public void move_piece(Cell cel0, Cell cel1) {
			if(cel0 == null || cel1 == null) { return; }

			if(cel0.occupied_by == null) { return; }

			cel1.occupied_by = cel0.occupied_by;
			cel0.occupied_by = null;

			cel1.occupied_by.x = cel1.col;
			cel1.occupied_by.y = cel1.row;
		}
	}


	public class Cell {
		public int col;
		public int row;
		public ChessPiece? occupied_by;

		public Cell(int col, int row) {
			this.col = col;
			this.row = row;
		}
	}


	interface IChessPiece {
		public List<int[]> find_legal_moves();
	}
	public class ChessPiece : IChessPiece {
		public int x, y, color;
		public List<int[]> legal_moves = new List<int[]>();

		public ChessPiece(int x, int y, int color) {
			this.color = color;
			this.x = x;
			this.y = y;
		}

		public bool is_in_borders(int x, int y) {
			return !(x > 7 || x < 0 || y > 7 || y < 0);
		}

		public virtual List<int[]> find_legal_moves() {
			legal_moves.Clear();
			return legal_moves;
		}

		protected void add_if_empty(int x, int y) {
			if(!is_in_borders(x, y)) { return; }

			if(Program.ches.Board[x, y].occupied_by == null) {
				legal_moves.Add(new int[] { x, y });
			}
		}
		protected void add_if_enemy(int x, int y, int team) {
			if(!is_in_borders(x, y)) { return; }

			if(Program.ches.Board[x, y].occupied_by != null) {
				if(Program.ches.Board[x, y].occupied_by.color != team) {
					legal_moves.Add(new int[] { x, y });
				}
			}
		}

		protected void add_unless_friend(int x, int y, int team) {
			add_if_empty(x, y);
			add_if_enemy(x, y, team);
		}
	}


	public class Pawn : ChessPiece {
		public Pawn(int x, int y, int color) : base(x, y, color) { }

		public override List<int[]> find_legal_moves() {
			base.find_legal_moves();
			int dir = (color == 0) ? 1 : -1;

			// 1 ileri
			add_if_empty(x, y + dir);

			// ilk hareketteki 2. ileri
			if(color == 0 && y == 1 || color == 1 && y == 6) {
				if(Program.ches.Board[x, y + dir].occupied_by == null) {
					add_if_empty(x, y + 2 * dir);
				}
			}

			// çaprazlar
			add_if_enemy(x + 1, y + dir, color);
			add_if_enemy(x - 1, y + dir, color);

			return legal_moves;
		}
	}


	public class Rook : ChessPiece {
		public Rook(int x, int y, int color) : base(x, y, color) { }

		public override List<int[]> find_legal_moves() {
			base.find_legal_moves();

			// aþaðý
			for(int i = 1; i < 8; i++) {
				if(!is_in_borders(x, y + i)) { break; }

				add_if_empty(x, y + i);
				if(Program.ches.Board[x, y + i].occupied_by != null) {
					add_if_enemy(x, y + i, color);
					break;
				}
			}
			// yukarý
			for(int i = (-1); i > -8; i--) {
				if(!is_in_borders(x, y + i)) { break; }
				add_if_empty(x, y + i);
				if(Program.ches.Board[x, y + i].occupied_by != null) {
					add_if_enemy(x, y + i, color);
					break;
				}
			}


			// sað
			for(int i = 1; i < 8; i++) {
				if(!is_in_borders(x + i, y)) { break; }

				add_if_empty(x + i, y);
				if(Program.ches.Board[x + i, y].occupied_by != null) {
					add_if_enemy(x + i, y, color);
					break;
				}
			}
			// sol
			for(int i = (-1); i > -8; i--) {
				if(!is_in_borders(x + i, y)) { break; }

				add_if_empty(x + i, y);
				if(Program.ches.Board[x + i, y].occupied_by != null) {
					add_if_enemy(x + i, y, color);
					break;
				}
			}

			return legal_moves;
		}
	}


	public class Bishop : ChessPiece {
		public Bishop(int x, int y, int color) : base(x, y, color) { }

		public override List<int[]> find_legal_moves() {
			base.find_legal_moves();

			// aþaðý sað
			for(int i = 1; i < 8; i++) {
				if(!is_in_borders(x + i, y + i)) { break; }

				add_if_empty(x + i, y + i);
				if(Program.ches.Board[x + i, y + i].occupied_by != null) {
					add_if_enemy(x + i, y + i, color);
					break;
				}
			}
			// yukarý sol
			for(int i = (-1); i > -8; i--) {
				if(!is_in_borders(x + i, y + i)) { break; }
				add_if_empty(x + i, y + i);
				if(Program.ches.Board[x + i, y + i].occupied_by != null) {
					add_if_enemy(x + i, y + i, color);
					break;
				}
			}

			// aþaðý sol
			for(int i = 1; i < 8; i++) {
				if(!is_in_borders(x - i, y + i)) { break; }

				add_if_empty(x - i, y + i);
				if(Program.ches.Board[x - i, y + i].occupied_by != null) {
					add_if_enemy(x - i, y + i, color);
					break;
				}
			}
			// yukarý sað
			for(int i = 1; i < 8; i++) {
				if(!is_in_borders(x + i, y - i)) { break; }
				add_if_empty(x + i, y - i);
				if(Program.ches.Board[x + i, y - i].occupied_by != null) {
					add_if_enemy(x + i, y - i, color);
					break;
				}
			}
			
			return legal_moves;
		}
	}


	public class Knight : ChessPiece {
		public Knight(int x, int y, int color) : base(x, y, color) { }
		public override List<int[]> find_legal_moves() {
			base.find_legal_moves();

			//yukarý
			add_unless_friend(x - 1, y - 2, color);
			add_unless_friend(x + 1, y - 2, color);

			//aþaðý
			add_unless_friend(x - 1, y + 2, color);
			add_unless_friend(x + 1, y + 2, color);

			//sað
			add_unless_friend(x + 2, y - 1, color);
			add_unless_friend(x + 2, y + 1, color);

			//sol
			add_unless_friend(x - 2, y - 1, color);
			add_unless_friend(x - 2, y + 1, color);

			return legal_moves;
		}
	}


	public class King : ChessPiece {
		public King(int x, int y, int color) : base(x, y, color) { }
		public override List<int[]> find_legal_moves() {
			base.find_legal_moves();
			// yukarý
			add_unless_friend(x, y - 1, color);
			// yukarý sað
			add_unless_friend(x + 1, y - 1, color);
			// yukarý sol
			add_unless_friend(x - 1, y - 1, color);

			// sol
			add_unless_friend(x - 1, y, color);
			// sað
			add_unless_friend(x + 1, y, color);

			// aþaðý
			add_unless_friend(x, y + 1, color);
			// aþaðý sað
			add_unless_friend(x + 1, y + 1, color);
			// aþaðý sol
			add_unless_friend(x - 1, y + 1, color);

			return legal_moves;
		}
	}


	public class Queen : ChessPiece {
		Bishop b = new Bishop(9, 9, 0);
		Rook r = new Rook(9, 9, 0);
		public Queen(int x, int y, int color) : base(x, y, color) {
			b = new Bishop(x, y, color);
			r = new Rook(x, y, color);
		}
		public override List<int[]> find_legal_moves() {
			base.find_legal_moves();
			b.x = x; b.y = y;
			r.x = x; r.y = y;
			r.color = color;
			b.color = color;

			foreach(var item in b.find_legal_moves()) {
				legal_moves.Add(item);
			}

			foreach(var item in r.find_legal_moves()) {
				legal_moves.Add(item);
			}

			return legal_moves;
		}
	}


}
