#include <iostream>
#include <vector>
#include <unordered_map>
using namespace std;
bool containsNearbyDuplicate(std::vector<int>& nums, int k) {
    unordered_map<int, int> numIndexMap;

    for (int i = 0; i < nums.size(); i++) {
        if (numIndexMap.find(nums[i]) != numIndexMap.end() && (i - numIndexMap[nums[i]]) <= k) {
            return true;  // Found a duplicate within the allowed range
        }
        numIndexMap[nums[i]] = i;
    }

    return false;  // No duplicates found within the allowed range
}

int main() {
    vector<int> example;
    int n;
    cout << "input -1 to end your input"<<endl;
    cin >> n;
    while (n != -1) {
        example.push_back(n);
        cin >> n;
    }
    cout << "k=";
    int k;
    cin >> k;
    cout << (containsNearbyDuplicate(example,k) ? "true" : "false") << endl;

    return 0;
}
