@echo off
for /l %%i in (1,1,13) do (
    date 2021/10/%%i
    echo 2021/10/%%i > .Netx
    git add .
    git commit -m "auto commit date"
)
git push
