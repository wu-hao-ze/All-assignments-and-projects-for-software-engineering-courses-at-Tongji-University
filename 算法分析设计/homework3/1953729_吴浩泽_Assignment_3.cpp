int matrixChainMultiplication(vector<int> const &dims)
{
	int n = dims.size();
	int dp[n + 1][n + 1];
	for (int i = 1; i <= n; i++) {
		dp[i][i] = 0;
	}
	for (int len = 2; len <= n; len++) {
		for (int i = 1; i <= n - len + 1; i++) {
			int j = i + len - 1;
			dp[i][j] = INT_MAX;
			for (int k = i; j < n && k <= j - 1; k++) {
				int cost = dp[i][k] + dp[k + 1][j] + dims[i - 1] * dims[k] * dims[j];
				if (cost < dp[i][j]) {
					dp[i][j] = cost;
				}
			}
		}
	}
	return dp[1][n - 1];
} 
