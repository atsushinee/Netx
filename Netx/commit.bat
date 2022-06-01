@echo off
for /l %%i in (1,1,30) do (
    date 2022/6/%%i
    echo 2022/6/%%i > .Netx
    git add .
    git commit -m "auto commit date"
)
git push 
