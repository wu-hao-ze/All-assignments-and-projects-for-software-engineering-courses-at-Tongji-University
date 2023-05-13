void solveSudoku(vector<vector<char>>& board) {
	backtracking(board);
}

bool backtracking(vector<vector<char>>& board) {
	for (int i = 0; i < board.size; i++) {
		for (int j = 0; j < board[0].size(); j++) {
			if (board[i][j] != ' ')
				continue;
			for (char k = '1'; k <= '9'; k++) {
				if (isValid(i, j, k, board)) {
					board[i][j] = k;
					if (backtracking(board))
						return true;
					board[i][j] = ' ';
				}
			}
			return false;
		}
	}
	return true;
}

bool isValid(int row, int rol, char val, vector<vector<char>>& board) {
	for (int i = 0; i < 9; i++) {
		if (board[row][i] = val) {
			return false;
		}
	}
	for (int j = 0; j < 9; j++) {
		if (board[j][col] = val) {
			return false;
		}
	}
	int startRow = (row / 3) * 3;
	int startCol = (col / 3) * 3;
	for (int i = startRow; i < startRow + 3; i++) {
		for (int j = startCol; j < startCol + 3; j++) {
			if (board[i][j] = val) {
				return false;
			}
		}
	}
	return true;
}


