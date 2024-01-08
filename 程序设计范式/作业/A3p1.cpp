#include <iostream>
#include <vector>
#include <unordered_set>
using namespace std;

bool containsDuplicate(vector<int>& nums) {
    unordered_set<int> numSet;

    for (int num : nums) {
        if (numSet.find(num) != numSet.end()) {
            return true;  // Found a duplicate
        }
        numSet.insert(num);
    }

    return false;  // No duplicates found
}

int main() {
    vector<int> example ;
    int n;
    cout << "input -1 to end your input"<<endl;
    cin >> n;
    while (n != -1) {
        example.push_back(n);
        cin >> n;
    }
    cout << (containsDuplicate(example) ? "true" : "false") << endl;

    return 0;
}
