import { defineConfig } from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  lang: 'zh-CN',
  titleTemplate: 'VNGod',
  title: "VNGod",
  description: "简洁有用的视觉小说管理器",
  sitemap: {
    hostname: 'https://vngod.samhou.moe'
  },
  lastUpdated: true,
  markdown: {
    lineNumbers: true
  },
  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [
      { text: '主页', link: '/' },
      { text: '文档', link: '/quick-start/description' },
      { text: '下载', link: '/download' },
      { text: '讨论群',link: 'https://t.me/+s38gO1NIdw0zNTI1'}
    ],
    logo: 'favicon.ico',
    sidebar: [
      {
        text: '从这里开始',
        items: [
          { text: '简介', link: '/quick-start/description' },
          { text: '快速开始', link: '/quick-start/start' }
        ]
      },
      {
        text: '使用指南',
        items: [
          {text: '修改游戏信息',link:'/feature/game-edit'},
          {text: '图标搜刮',link:'/feature/icon'},
          {text: '游戏管理',link:'/feature/game-store'},
          {text: '隐藏游戏',link:'/feature/game-hide'},
          {text: '调试',link:'/feature/debug'},
        ]
      },
      {
        text: '开发',
        items: [
          { text: '开发环境', link: '/community/development' },
          { text: '参与贡献', link: '/community/contribute' }
        ]
      }
    ],
    socialLinks: [
      { icon: 'github', link: 'https://github.com/SamHou0/VNGod' },
      { icon: 'telegram', link: 'https://t.me/PatriciaBySamHou' },
      { icon: 'rss', link: 'https://blog.samhou.top' }
    ]
  }
})