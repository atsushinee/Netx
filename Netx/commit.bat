@echo off
for /l %%i in (1,1,30) do (
    date 2021/5/%%i
    echo 2021/5/%%i > .Netx
    git add .
    git commit -m "auto commit date"
)
git push
