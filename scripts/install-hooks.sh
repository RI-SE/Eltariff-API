#!/bin/sh

for hook_script in scripts/hooks/*; do
  if [ -x "$hook_script" ]; then
    hook_name=$(basename "$hook_script")
    echo "→ Installing $hook_name hook..."

    cp "$hook_script" ".git/hooks/$hook_name"
    chmod +x ".git/hooks/$hook_name"

    if [ -f ".git/hooks/$hook_name" ]; then
      echo "✅ $hook_name hook installed!"
    else
      echo "❌ ERROR: Failed to install the $hook_name hook!"
      exit 1
    fi
  fi
done

echo "✅ All executable Git hooks installed successfully."
