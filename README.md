# DotnetPatch
用于对dotnet文件的方法进行替换或修改。

替换是指用同一个Assembly里的另一个类的方法体替换指定类的相同签名方法，此时只是将被修改方法的方法体指向替换的方法体，只适用于静态方法与不访问实例变量的方法。
在进行替换前通常应该使用IlMerge将替换类与被替换类的Assembly合并到一起（此工具为微软开发，bin/Release下的IlMerge.exe即是）。

对方法的修改主要是插入调用指令，此时通过基于dsl的简单脚本来处理，示例如下：

proc(main)
{
	$files = getfilelist();
		begin("开始脚本处理");
		looplist($files){
		$file=$$;
		beginfile($file,"开始对"+$file+"进行修改。。。");
		beginreplace($file);
		replace($file,"Game","GamePatch");
		endreplace($file);
		beginmodify($file);
		writeloadarg($file,"PlayerController","Update",0x38,0);
		writeloadfield($file,"PlayerController","Update",0x39,"PlayerController","m_player");
		writeloadarg($file,"PlayerController","Update",0x3e,0);
		writeloadfield($file,"PlayerController","Update",0x3f,"PlayerController","m_moveElapseTime");
		writecall($file,"PlayerController","Update",0x44,"DebugConsoleHelper","Move");
		writenops($file,"PlayerController","Update",0x49,0x22);
		endmodify($file);
		endfile($file);
	};
	end("结束脚本处理");
};

脚本处理时首先取到由玩家添加的待处理的文件，然后依次对各文件进行处理，脚本处理可以进行方法替换、扩展某个方法的大小或对某个方法的指令进行修改。
脚本支持的命令有：

    begin(string tip);	//开始脚本处理
    end(string tip);	//结束脚本处理
    beginfile(string file, string tip);	//开始处理文件
    endfile(string file);	//结束处理文件
    beginreplace(string file);	//开始方法体替换
    replace(string file, string srcclass, string repclass);	//替换方法体
    endreplace(string file);	//结束方法体替换
    beginextend(string file);	//开始扩展方法大小
    extend(string file, string classname, string methodname, uint insertsize); //在指定的方法开始插入指定字节数
    endextend(string file);	//结束方法大小扩展
    beginmodify(string file);	//开始文件方法修改
    endmodify(string file);	//结束文件方法修改
    
    writeloadarg(string file, string classname, string methodname, uint pos, int index);	//写入loadarg指令
    writeloadlocal(string file, string classname, string methodname, uint pos, int index);	//写入loadlocal指令
    writeloadfield(string file, string classname, string methodname, uint pos, string loadClass, string loadField);	//写入loadfield指令
    writeloadstaticfield(string file, string classname, string methodname, uint pos, string loadClass, string loadField);	//写入loadstaticfield指令
    writecall(string file, string classname, string methodname, uint pos, string callClass, string callMethod);	//写入call指令
    writecallvirt(string file, string classname, string methodname, uint pos, string callClass, string callMethod);	//写入callvirt指令
    writenops(string file, string classname, string methodname, uint pos, int size);	//写入多个nop指令
    
    getfilelist();	//获取当前添加的待处理文件列表
    log(format[,arg1,arg2,...]);	//输出日志到错误信息窗口，格式与参数的规则与string.Format方法相同
    
另外，基于dsl的简单计算脚本支持如下命令：

		运算：+ - * / % () ?:
		比较：> >= < <= == !=
		逻辑：&& || !
		函数：max min abs clamp
		语句：if while loop looplist foreach
		位置变量与参数访问：arg(index) var(index)
		赋值：‘变量名 = 值’ 或 ‘var(index) = 值’
		
		if(条件表达式){
			语句列表;
		}elseif(条件表达式){ //elseif可以有0到多个
			语句列表;		
		} else {	//else可以有0或1个
			语句列表;
		};
		
		while(条件表达式){
			语句列表;
		};
		
		loop(次数){
			$$ 为当前次数
			语句列表;
		};
		
		looplist(list){
			$$ 为列表遍历到的当前无素
			语句列表;
		};
		
		foreach(e1,e2,...){
			$$ 为列表遍历到的当前无素
			语句列表;
		};
		
注意：脚本基于dsl元语言，所以语法上每个语句结束都需要加分号！