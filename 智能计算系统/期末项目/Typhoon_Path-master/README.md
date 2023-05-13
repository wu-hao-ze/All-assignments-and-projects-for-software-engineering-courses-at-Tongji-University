# Typhoon_Path
Official source code for paper 《AM-ConvGRU: A Spatio-Temporal Model for Typhoon Path Prediction》

## To run the code please kindly follow the steps below

* Step 1. Install the requirement environment 
```
conda env create -f Typhoon_Path.yaml
```  
---

* Step 2. Download the require z (geopotential) reanalysis data from ERA-Interim offical site
<https://apps.ecmwf.int/datasets/data/interim-full-daily/levtype=sfc/>
or download the preprocessing data from my google drive
<https://drive.google.com/drive/folders/1o6afeGE9cux5uGB5j0Tzot_ygYAOsxv3?usp=sharing>

---

* Step 3. When the download process finishes, pleas put the files into the directory **./data/ERA_Interim/**

---

* Step 4. Run the jupyter notebook **3D_Typhoon_features_constructor.ipynb** in the ERA_Interim folder to construct the 3D typhoon structure in time-lots

---

* Step 5. Finnaly, you can run the main jupyter notebook **Model.ipynb**

## Overall Architecture of our propsed model

![image](https://github.com/xuguangning1218/Typhoon_Path/blob/master/figure/network.png)

## Citation
If you find this code or idea useful, please cite our work:
```bib
@inproceedings{xu2022am,
  title={AM-ConvGRU: a spatio-temporal model for typhoon path prediction},
  author={Xu, G., Xian, D., Fournier-Viger, P., Xutao, L, Yunming, Y, Xiuqing H},
  JO={Neural Computing and Applications},
  volume={34},
  pages={5905–5921},
  dio={https://doi.org/10.1007/s00521-021-06724-x}
  year={2022}
}
```
