@echo off
for /l %%i in (1,1,31) do (
    date 2020/12/%%i
    echo 2020/12/%%i > .Netx
    git add .
    git commit -m "auto commit date"
)
git push
