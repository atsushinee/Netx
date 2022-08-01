@echo off
for /l %%i in (1,3,30) do (
    date 2022/8/%%i
    echo 2022/8/%%i > .Netx
    git add .
    git commit -m "auto commit date"
)
@REM git push 
