 #framework镜像
FROM mcr.microsoft.com/dotnet/framework/runtime:4.7.2

#作者
#2020-03-25
MAINTAINER vongzh@qq.com

#工作目录
WORKDIR /api

#当前镜像为runtime
#此处编译需要sdk
#COPY *.csproj ./
#RUN dotnet restore

#COPY . ./
#RUN dotnet publish -c Release -o api

#此处镜像为runtime时拷贝本地直接发布
#复制文件
COPY /bin/x64/Debug /api

#开放端口
EXPOSE 8555

#运行
ENTRYPOINT ["Zk.HotelPlatform.Api.exe"]

#以下为启动命令
#docker build -t hotelplatformapi:dev .
#docker run -d -p 8555:8555 --name hotelplatformapi hotelplatformapi:dev

#查询容器IP
#docker inspect hotelplatformapi --format='{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}'