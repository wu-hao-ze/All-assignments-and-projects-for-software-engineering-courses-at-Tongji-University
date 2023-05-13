# Typhoon Intensity Prediction
Official source code for paper 《SAF-Net: A Spatio-Temporal Deep Learning Method for Typhoon Intensity Prediction》

## To run the code please kindly follow the steps below

* Step 1. Install the requirement environment 
```
conda env create -f TI_Prediction.yaml
```  
---

* Step 2. Download the require u and v component wind speed reanalysis data from ERA-Interim offical site
<https://apps.ecmwf.int/datasets/data/interim-full-daily/levtype=sfc/>
or download the preprocessing data from my google drive
<https://drive.google.com/drive/folders/1jxgoTwUjIELgoTHPiyT183UTIvOY6vU3?usp=sharing>

---

* Step 3. When the download process finishes, pleas put the files into the directory **./data/ERA_Interim/**

---

* Step 4. Run the jupyter notebook **3D_Typhoon_Structure_Constructed_In_Time_lots.ipynb** in the ERA_Interim folder to construct the 3D typhoon structure in time-lots

---

* Step 5. Finnaly, you can run the main jupyter notebook **SAF-Net.ipynb**

## Overall Architecture of SAF-Net

![image](https://github.com/xuguangning1218/TI_Prediction/blob/master/figure/Model.png)

If you think our work is helpful. Please kindly cite
```
@article{XU2022121,
title = {SAF-Net: A spatio-temporal deep learning method for typhoon intensity prediction},
journal = {Pattern Recognition Letters},
volume = {155},
pages = {121-127},
year = {2022},
issn = {0167-8655},
doi = {https://doi.org/10.1016/j.patrec.2021.11.012},
url = {https://www.sciencedirect.com/science/article/pii/S0167865521004037},
author = {Guangning Xu and Kenghong Lin and Xutao Li and Yunming Ye},
}
```
