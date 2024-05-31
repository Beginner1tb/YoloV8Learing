## 注意事项
### 1.使用环境
#### 1.CUDA和cuDNN版本
CUDA版本为11.X，粗DNN版本为8.X
#### 2.OnnxRuntime版本
工程需要引入OnnxRuntime.GPU和OnnxRuntime.Managed，最高版本为17.3，最低测试16.0可以通过，18.0以上无法使用上述CUDA和cuDNN环境下的GPU
#### 3.使用流程
1. 从文件载入onnx模型，并选择是否使用GPU，注意，模型载入是一次性的，后续更改模型或者修改是否使用GPU都要重新载入；
2. 获取图像路径；
3. 进行推理，并显示结果
