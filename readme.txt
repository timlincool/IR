使用方式：
將SPLIT_DOC_WDID_NEW資料夾
QUERY_WDID_NEW資料夾
AssessmentTrainSet.txt放至c:\下

SPLIT_DOC_WDID_NEW與QUERY_WDID_NEW為語音資料
使用page ranking演算法來計算語音資料排名

先運行Rocchio Method資料夾程式，使用演算法Rocchio Method後
，將運算結果在c:\下產生ResultsTrainSetrelevent.txt


最後運行IR資料夾程式，使用平均準確率(mean Average Precision, mAP)算出結果。


撰寫語言為C# 與 c++
