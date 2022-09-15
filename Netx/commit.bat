@echo off
for /l %%i in (1,2,15) do (
    date 2022/9/%%i
    echo 2022/9/%%i > .Netx
    git add .
    git commit -m "auto commit date"
)
@REM git push 
