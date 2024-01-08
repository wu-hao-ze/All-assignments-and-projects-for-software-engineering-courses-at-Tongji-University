#include <iostream>
#include <vector>

void calculateNextPriceIncreases(const std::vector<int>& prices, std::vector<int>& answer) {
    int n = prices.size();
    for (int i = 0; i < n; ++i) {
        int daysUntilIncrease = 0;
        for (int j = i + 1; j < n; ++j) {
            if (prices[j] > prices[i]) {
                daysUntilIncrease = j - i;
                break;
            }
        }
        answer[i] = daysUntilIncrease;
    }
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
    int n = prices.size();

    std::vector<int> answer(n);

    calculateNextPriceIncreases(prices, answer);

    // Output result
    for (int i = 0; i < n; ++i) {
        std::cout << answer[i] << " ";
    }
    std::cout << std::endl;

    return 0;
}
