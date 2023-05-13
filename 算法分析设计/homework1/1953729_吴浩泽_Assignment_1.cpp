#include <bits/stdc++.h>
using namespace std;

const int N = 10010;
int dp[N];

class Solution {
public:
    int coinChange(vector<int>& coins, int amount) {
        int len = coins.size();

        memset(dp, 0, sizeof(dp));

        dp[0] = 0;
        for (int i = 1; i <= amount; i++) dp[i] = amount + 1;

        for (int i = 1; i <= amount; i++) {
            for (int j = 0; j < len; j++) {
                if (i >= coins[j]) {
                    dp[i] = min(dp[i], dp[i - coins[j]] + 1);
                }
            }
        }

        return dp[amount] == amount + 1 ? -1 : dp[amount];
    }
};