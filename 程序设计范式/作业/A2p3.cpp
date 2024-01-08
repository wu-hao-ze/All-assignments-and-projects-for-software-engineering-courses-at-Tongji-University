#include <iostream>
#include <vector>
#include <algorithm>

using namespace std;

vector<vector<int>> mergeIntervel(vector<vector<int>>& intervals) {
    int n = intervals.size();
    for (int i = 0; i < n - 1; ++i) {
        for (int j = 0; j < n - i - 1; ++j) {
            if (intervals[j][0] > intervals[j + 1][0]) {
                swap(intervals[j], intervals[j + 1]);
            }
        }
    }
    vector<vector<int>> mergedIntervals;
    mergedIntervals.push_back(intervals[0]); 

    for (int i = 1; i < intervals.size(); ++i) {
        vector<int>& current = intervals[i];
        vector<int>& previous = mergedIntervals.back(); 

        if (current[0] <= previous[1]) {
            
            previous[1] = max(previous[1], current[1]);
        }
        else {
           
            mergedIntervals.push_back(current);
        }
    }
    return mergedIntervals;
}

int main() {
    int n;
    cout << "Enter the number of movies(-1 to exit): " << endl;
    while (cin >> n) {
        if (n == -1) { break; }
        cout << "Enter the start and end times of each movie: " << endl;
        vector<vector<int>> intervals;
        for (int i = 0; i < n; i++) {
            int a, b;
            cin >> a >> b;
            vector<int> temp;
            temp.push_back(a);
            temp.push_back(b);
            intervals.push_back(temp);
        }
        cout << "[";
        for (int i = 0; i < mergeIntervel(intervals).size(); ++i) {
            cout <<" [" << mergeIntervel(intervals)[i][0]<<","<< mergeIntervel(intervals)[i][1]<<"] ";
        }
        cout<<"]" << endl;
        
        cout << "Enter the number of movies(-1 to exit): " << endl;
    }

    return 0;
}