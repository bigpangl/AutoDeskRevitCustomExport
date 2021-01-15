##  AutoDeskRevitCustomExport

此项目当Revit 插件的形式，实现BIM 模型几何信息的导出。

## 重点解决问题

1. 除几何信息以外,其他数据的缺失。
  
格式上,导出时以Element为单元存放数据,存放几何信息以外的其他信息变得容易。
例如,以前存放整个几何信息的格式为:
    
```
{
    "vertexs":[
            
    ],
    "angle":[
    ],
    "normal":[]
}
```
现在为:
    
```
{
    "elements":[
        {
            "id":1,
            "meshs":[
                    {
                        "vertexs":[
            
                        ],
                        "angle":[
                        ],
                        "normal":[]
                    }
            ]
        }
    ]
}
``` 
如此一来,为提取和Revit 中Element其他属性的参数提供了一个兼容的基础条件

2. 导出出数据量大
    
有时候,导出的几何数据会非常大,甚至远远大于Revit 的BIM 模型文件本身,发生内存占用过高程序崩溃等等问题。

此时有两种可能,第一种,模型文件本身较大,内容较多,第二种可能,就是模型文件本身复用关系较多,例如导出时未利用族实例相同这一特点.

针对前者,通过编程技巧和格式制定上,以element分文件存储,导出到指定文件夹.

对于后者,在格式制定时,就包含了复用关系.可以通过旋转矩阵的记录,缩小需要保存的数据的内容.
    
例如:
    
```
//单个element中

{
            "id":1,
            "meshs":[
                    {
                        "vertexs":[
            
                        ],
                        "angle":[
                        ],
                        "normal":[]
                    }
            ],
            "instances":[
                {
                    "id",
                    "rotate":[1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1] // 旋转矩阵
                }
            ]
        }
```
使用的旋转矩阵不包含缩放功能


