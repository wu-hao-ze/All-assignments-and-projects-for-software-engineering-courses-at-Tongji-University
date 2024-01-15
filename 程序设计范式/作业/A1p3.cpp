#include <iostream>
#include <vector>

int maxProfit(std::vector<int>& prices) {
    int n = prices.size();
    if (n <= 1) {
        return 0;
    }

    std::vector<int> buy(n, 0);
    std::vector<int> sell(n, 0);

    buy[0] = -prices[0];  // Buy on the first day

    for (int i = 1; i < n; ++i) {
        // On each day, you can either:
        // 1. Stay in the same state (cooldown or do nothing)
        // 2. Buy a stock (if not already bought)
        // 3. Sell a stock (if already bought)

        // Update buy[i]
        // You can buy on day i if you didn't buy on day i-1 or you cooldown on day i-1
        buy[i] = std::max(buy[i - 1], sell[i - 2] - prices[i]);

        // Update sell[i]
        // You can sell on day i if you bought on day i-1 or you do nothing on day i-1
        sell[i] = std::max(sell[i - 1], buy[i - 1] + prices[i]);
    }

    // The maximum profit will be in the sell[n-1] state
    return sell[n - 1];
}

int main() {
    std::vector<int> prices = { 1, 2, 3, 0, 2 };
    int profit = maxProfit(prices);
    std::cout << profit << std::endl;  // Output: 3

    return 0;
}
