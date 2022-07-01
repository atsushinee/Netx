@echo off
for /l %%i in (1,1,30) do (
    date 2022/7/%%i
    echo 2022/7/%%i > .Netx
    git add .
    git commit -m "auto commit date"
)
@REM git push 
