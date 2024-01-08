#include <iostream>
#include <vector>
using namespace std;
bool containsNearbyAlmostDuplicate(vector<int>& nums, int indexDiff, int valueDiff) {
    for (int i = 0; i < nums.size(); i++) {
        for (int j = i + 1; j < nums.size(); j++) {
            if (i != j && abs(i - j) <= indexDiff && abs(nums[i] - nums[j]) <= valueDiff) {
                return true;  // Found satisfying indices
            }
        }
    }

    return false;  // No satisfying indices found
}

int main() {
    vector<int> example;
    int n;
    cout << "input -1 to end your input" << endl;
    cin >> n;
    while (n != -1) {
        example.push_back(n);
        cin >> n;
    }
    cout << "indexDiff=";
    int i;
    cin >> i;
    cout<< "valueDiff=";
    int v;
    cin >> v;
    cout << (containsNearbyAlmostDuplicate(example,i,v) ? "true" : "false") << endl;

    return 0;
}
