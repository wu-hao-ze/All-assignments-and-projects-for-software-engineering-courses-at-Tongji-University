#include <iostream>
#include <vector>

int maxProfit(std::vector<int>& prices) {
    if (prices.empty()) {
        return 0;
    }

    int max_profit = 0;  // Initialize the maximum profit to 0
    int min_price = prices[0];  // Initialize the minimum price to the first price

    for (int price : prices) {
        min_price = std::min(min_price, price);  // Update the minimum price
        max_profit = std::max(max_profit, price - min_price);  // Calculate the maximum profit
    }

    return max_profit;
}

int main() {
    std::vector<int> prices;
    int price;

    std::cout << "Enter stock prices followed by -1 to finish input:" << std::endl;
    while (true) {
        std::cin >> price;
        if (price == -1) {
            break;  // Enter -1 to finish input
        }

        prices.push_back(price);
    }
    int profit = maxProfit(prices);
    std::cout << profit << std::endl;  

    return 0;
}
