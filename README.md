# AosFileTransfer
작성자 : 김응기

- Linux 시스템으로 파일을 자동 전송하는 프로그램
- 기존의 FileZilla를 통하지 않고 소프트웨어 실행 한번으로 파일을 전송하는 컨셉.

## 전제조건 : 
1. 기본적으로 user 디렉토리에 Application 폴더 내에 실행 할 어플리케이션 폴더가 존재해야함.
현재 버전으론 Application 내에 폴더가 없을 시 생성하지 않음.

> [!IMPORTANT]
> ~/Application/Scanner_Blazor

2. AgosFileTransfer.exe.config
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8" />
    </startup>
	<appSettings>
		<!-- 변수 정의 -->
		<add key="FtpServerUrl" value="ftp://" />
		<add key="UserName" value="" />
		<add key="Password" value="" />
		<add key="Folder" value="Scanner_Blazor"/>		
		<!--<add key="Folder" value="SpiderShield_Blazor"/>-->
	</appSettings>
</configuration>
```

위의 파일은 git에 올려놓지 않았으므로 직접 생성할 것.

FtpServerUrl, UserName, Password 를 모른다면 작성자 본인에게 문의 할 것.

Folder는 현시점에 Scanner_Blazor, SpiderShield_Blazor 두 개만 존재함.
(추후에 더 많은 Ap plication 이 생긴다면 소스코드를 수정해야 할 필요가 있음.)

---

## 실제 사용 방법

1. 위와 같이 전송할 폴더 바깥에 해당 소프트웨어를 위치 시킨다.
(visual studio 내 publish로 셋팅 되는 폴더 바깥에 위치 시켜 놓으면, 다시 셋팅 할 필요가 없음.)
2. config 에서 Folder를 전송할 Folder명과 동일하게 맞춰준다.
(절대 놓치면 안됨.)
3. 전송 완료 시 ssh 로 접속하여 systemctl daemon-reload와 systemctl restart {service}를 실행해준다.
